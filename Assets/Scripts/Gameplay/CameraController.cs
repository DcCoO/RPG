using System;
using System.Collections;
using BitBox.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace BitBox.Gameplay
{
    [DefaultExecutionOrder(10)]
    public class CameraController : SingletonMonoBehaviour<CameraController>
    {
        [FoldoutGroup("References"), SerializeField] private Camera _camera;
        [FoldoutGroup("References"), SerializeField] private Transform _player;
        [FoldoutGroup("References"), SerializeField] private RawImage _rawImage;
        [FoldoutGroup("References"), SerializeField] private Transform _targetToFollow;

        [FoldoutGroup("Information", true), SerializeField, ReadOnly] private Vector3 _exactPosition;
        [FoldoutGroup("Information", true), SerializeField, ReadOnly] private Vector3 _snappedPosition;
        [FoldoutGroup("Information", true), SerializeField, ReadOnly] private bool _isRotating;
        [FoldoutGroup("Information", true), SerializeField, ReadOnly] private bool _is45Rotating;
        [FoldoutGroup("Information", true), SerializeField, ReadOnly] private Vector2 _pixelSize;
        [FoldoutGroup("Information", true), SerializeField, ReadOnly] private Vector2 _worldSize;
        //[FoldoutGroup("Information", true), SerializeField, ReadOnly] private bool _hasTargetLock;

        [FoldoutGroup("Settings", true), SerializeField] private float _inputRotationSpeed;
        [FoldoutGroup("Settings", true), SerializeField] private float _inputRotation45Duration;
        [FoldoutGroup("Settings", true), SerializeField] private float _focusRotationSpeed;
        [FoldoutGroup("Settings", true), SerializeField] private AnimationCurve _rotation45Curve;
        [FoldoutGroup("Settings", true), SerializeField] private bool _useSmoothMovement;

        private Vector3 _axisContribution;
        private Vector3 _axisUnitsPerPixel;
        private Vector3 _squaredSize;
        private Vector3 _scaledForward, _scaledRight, _scaledUp;
        private Transform _transform;

        public Vector3 Right => _transform.right;
        public Vector3 Up => _transform.up;
        public Vector3 Forward => _transform.forward;

        private void Start()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            _transform = transform;
            _exactPosition = _transform.position;
            _axisContribution = new Vector3(1, Mathf.Cos(_camera.transform.localEulerAngles.x * Mathf.Deg2Rad), Mathf.Sin(_camera.transform.localEulerAngles.x * Mathf.Deg2Rad));
            _axisUnitsPerPixel.x = (2f * _camera.orthographicSize) / 360f;
            _axisUnitsPerPixel.y = _axisUnitsPerPixel.x / _axisContribution.y;
            _axisUnitsPerPixel.z = _axisUnitsPerPixel.x / _axisContribution.z;
            _squaredSize = Vector3.Scale(_axisUnitsPerPixel, _axisUnitsPerPixel);
            _pixelSize = new Vector2(1f / Screen.width, 1f / Screen.height);
            var screenRatio = Screen.width / (float)Screen.height;
            _worldSize = new Vector2(2 * _camera.orthographicSize * screenRatio, 2 * _camera.orthographicSize);
        }

        private void FixedUpdate()
        {
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            _exactPosition = _targetToFollow.position;
            _snappedPosition = GetSnappedPosition(_exactPosition);

            // Movement
            if (_isRotating || _is45Rotating) _transform.position = _exactPosition;
            else _transform.position = _snappedPosition;

            /*if (_hasTargetLock)
            {
                RotateToFocus();
                return;
            }*/
            
            // 45 Rotation
            if (Input.GetKey(KeyCode.Q)) Rotate(true);
            if (Input.GetKey(KeyCode.E)) Rotate(false);

            // Continuous Rotation
            if (Input.GetKey(KeyCode.Z))
            {
                _transform.localEulerAngles += Vector3.up * (_inputRotationSpeed * Time.deltaTime);
                _isRotating = true;
            }
            else if (Input.GetKey(KeyCode.C))
            {
                _transform.localEulerAngles -= Vector3.up * (_inputRotationSpeed * Time.deltaTime);
                _isRotating = true;
            }
            else _isRotating = false;

            if (_useSmoothMovement && !_is45Rotating && !_isRotating) Smooth();
        }

        /*private void RotateToFocus()
        {
            _transform.rotation = Quaternion.Slerp(_transform.rotation, Quaternion.LookRotation(_player.forward),
                _focusRotationSpeed * Time.fixedDeltaTime);
        }

        [Button]
        private void ToggleTargetLock()
        {
            _hasTargetLock = !_hasTargetLock;
        }*/

        private void Rotate(bool clockwise)
        {
            if (_is45Rotating) return;
            _is45Rotating = true;
            StartCoroutine(RotateRoutine(clockwise));
        }

        private IEnumerator RotateRoutine(bool clockwise)
        {
            var localEulerAngles = _transform.localEulerAngles;
            var yf = localEulerAngles.y + (clockwise ? 45 : -45);

            for (var t = 0f; t < 1f; t += Time.deltaTime / _inputRotation45Duration)
            {
                _transform.localEulerAngles = new Vector3(localEulerAngles.x,
                    Mathf.Lerp(localEulerAngles.y, yf, _rotation45Curve.Evaluate(t)), localEulerAngles.z);
                yield return null;
            }

            _transform.localEulerAngles = new Vector3(localEulerAngles.x, yf, localEulerAngles.z);
            _is45Rotating = false;
        }

        private Vector3 GetSnappedPosition(Vector3 position)
        {
            _scaledRight = _transform.right * _axisUnitsPerPixel.x;
            _scaledUp = _transform.up * _axisUnitsPerPixel.y;
            _scaledForward = _transform.forward * _axisUnitsPerPixel.z;

            var x = Vector3.Dot(_scaledRight, position) / _squaredSize.x;
            var y = Vector3.Dot(_scaledUp, position) / _squaredSize.y;
            var z = Vector3.Dot(_scaledForward, position) / _squaredSize.z;

            return Mathf.Round(x) * _scaledRight + Mathf.Round(y) * _scaledUp + Mathf.Round(z) * _scaledForward;
        }

        private void Smooth()
        {
            var offset = _exactPosition - _snappedPosition;
            var projectedOffset = new Vector2(Vector3.Dot(offset, _transform.right),
                Vector3.Dot(offset, _transform.up) * _axisContribution.y +
                Vector3.Dot(offset, _transform.forward) * _axisContribution.z);

            var pixelsMoved = new Vector2(
                projectedOffset.x * Screen.width / _worldSize.x,
                projectedOffset.y * Screen.height / _worldSize.y
            );

            _rawImage.uvRect = new Rect(pixelsMoved.x * _pixelSize.x, pixelsMoved.y * _pixelSize.y, 1, 1);
        }
    }
}
