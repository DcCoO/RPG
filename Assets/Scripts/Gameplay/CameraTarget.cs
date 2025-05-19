using System;
using UnityEngine;

namespace BitBox.Gameplay
{
    public class CameraTarget : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _target;
        [SerializeField] private float _offset;
        [SerializeField, Range(0, 1)] private float _followSpeed;
        [SerializeField, Range(0f, 1f)] private float _playerTargetFocus;

        private Vector3 _velocity;
        private CameraController _cameraController;
        private bool _isLockedOnTarget;
        
        private void OnEnable()
        {
            TargetLockController.OnTargetLockStateChanged += UpdateTarget;
        }

        private void OnDisable()
        {
            TargetLockController.OnTargetLockStateChanged -= UpdateTarget;
        }

        private void Start()
        {
            _cameraController = CameraController.Instance;
        }

        private void FixedUpdate()
        {
            var targetPosition = _isLockedOnTarget ? Vector3.Lerp(_player.position, _target.position, _playerTargetFocus) : _player.position;
            
            var offset = _offset * _cameraController.Forward;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + offset, ref _velocity, _followSpeed);
        }

        private void UpdateTarget(bool isLocked, Transform target)
        {
            _target = target;
            _isLockedOnTarget = isLocked;
        }
    }
}