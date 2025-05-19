using System;
using System.Collections.Generic;
using System.Linq;
using BitBox.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BitBox.Gameplay
{
    /// <summary>
    /// This class is responsible for managing the target lock state in the game.
    /// It allows switching between locked and unlocked states and notifies subscribers of state changes.
    /// </summary>
    public class TargetLockController : SingletonMonoBehaviour<TargetLockController>
    {
        public static event Action<bool, Transform> OnTargetLockStateChanged;
        
        public Transform Player;
        public LockableEntity Target;
        public bool Locked;
        public RectTransform TargetIndicator;
        
        [BoxGroup("Settings"), SerializeField] private float _maxLockRadius;
        [BoxGroup("Settings"), SerializeField] private float _verticalOffset;
        [BoxGroup("Settings"), SerializeField] private float _indicatorSmoothTime;

        private Vector2 _indicatorVelocity;
        
        private Camera _camera;
        
#if UNITY_EDITOR
        public bool Debug;

        private void OnDrawGizmos()
        {
            if (!Debug) return;
            
            GizmosExtensions.DrawWireCylinder(Player.position, _maxLockRadius, 0.1f);
        }
#endif

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            // TODO: move to InputController
            if (Input.GetKeyDown(KeyCode.L))
            {
                Lock();
            }

            if (!Locked) return;
            
            var viewportPoint = _camera.WorldToViewportPoint(Target.LockPosition);
            var screenPoint = new Vector2(viewportPoint.x * Screen.width, viewportPoint.y * Screen.height);
            TargetIndicator.anchoredPosition = Vector2.SmoothDamp(TargetIndicator.anchoredPosition, screenPoint, ref _indicatorVelocity, _indicatorSmoothTime);

            if (Vector3.Distance(Player.position, Target.Position) > _maxLockRadius)
            {
                Unlock();
            }
        }

        private void Unlock()
        {
            if (Locked) OnTargetLockStateChanged?.Invoke(false, Player);
            Target = null;
            Locked = false;
            TargetIndicator.gameObject.SetActive(false);
        }
        
        private void Lock()
        {
            if (Locked)
            {
                Unlock();
                return;
            }

            var transforms = GetEntitiesInLockRadius();
            
            if (transforms.Count == 0) return;
            
            LockOnMostAlignedTarget(transforms);

            Locked = true;
            TargetIndicator.gameObject.SetActive(true);
            
            var viewportPoint = _camera.WorldToViewportPoint(Target.LockPosition);
            var screenPoint = new Vector2(viewportPoint.x * Screen.width, viewportPoint.y * Screen.height);
            TargetIndicator.anchoredPosition = screenPoint;
            
            OnTargetLockStateChanged?.Invoke(true, Target.transform);
        }

        private List<LockableEntity> GetEntitiesInLockRadius()
        {
            var colliders = Physics.OverlapSphere(Player.position, _maxLockRadius);
            var entities = new List<LockableEntity>();
            
            foreach (var col in colliders)
            {
                if (col.TryGetComponent(out LockableEntity entity)) entities.Add(entity);
            }
            return entities;
        }
        
        private void LockOnMostAlignedTarget(List<LockableEntity> targets)
        {
            var playerPosition = Player.position.XZ();
            var playerForward = Player.forward.XZ();
            float minAngle = 180f;
            
            foreach (var target in targets)
            {
                var direction = target.Position.XZ() - playerPosition;
                var angle = Vector3.Angle(playerForward, direction);
                
                if (angle < minAngle)
                {
                    minAngle = angle;
                    Target = target;
                }
            }
        }
    }
}