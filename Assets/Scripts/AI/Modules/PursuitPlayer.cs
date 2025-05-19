using System;
using BitBox.Utils;
using Pathfinding;
using UnityEngine;

namespace BitBox.AI
{
    [Serializable]
    public class PursuitPlayer : Module
    {
        [SerializeField] private float _followSpeed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _intervalBetweenRepaths = 0.4f;
        [SerializeField] private float _desiredDistanceFromTarget;

        //private Seeker _seeker;
        private FollowerEntity _follower;
        private Transform _transform;
        private Rigidbody _rigidbody;
        
        private Transform _playerTransform;

        private float _lastRepathTime;
        private int _currentWaypoint;

        private const float _distanceToReachWaypoint = 0.3f;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            RunsInUpdate = false;
            RunsInFixedUpdate = true;

            _transform = agent.transform;
            _rigidbody = agent.GetComponent<Rigidbody>();
            _follower = agent.GetComponent<FollowerEntity>();
            
            var player = Registry.Get(EAgent.Player);
            if (player is not null) _playerTransform = player.transform;

            _intervalBetweenRepaths = 0.4f;
            _lastRepathTime = float.NegativeInfinity;
        }

        public override void UpdateExecutableState()
        {
            CanExecute = Vector3.Distance(_transform.position, _playerTransform.position) > _desiredDistanceFromTarget;
        }

        public override void Execute()
        {
            base.Execute();
            _follower.maxSpeed = _followSpeed;
        }

        public override void FixedUpdate()
        {
            if (!IsExecuting) return;

            var currentDistance = Vector3.Distance(_transform.position, _playerTransform.position);

            if (currentDistance < _desiredDistanceFromTarget)
            {
                Stop();
                return;
            }
                
            if (Time.time > _lastRepathTime + _intervalBetweenRepaths)
            {
                _lastRepathTime = Time.time;
                _follower.destination = _playerTransform.position;
                _follower.SearchPath();
            }
            
            _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, Quaternion.LookRotation(_follower.velocity.normalized), _rotationSpeed * Time.fixedDeltaTime);
        }

        public override void Stop()
        {
            base.Stop();
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _follower.maxSpeed = 0;
        }

        public override void DrawGizmos()
        {
            if (_follower is null) return;

            Gizmos.color = Color.green;
            Gizmos.DrawRay(_transform.position, _follower.velocity.normalized);
        }

        public override Type[] GetRequiredComponents()
        {
            if (_hasAnimation) return new[] { typeof(Rigidbody), typeof(Seeker), typeof(Animator) };
            return new[] { typeof(Rigidbody), typeof(FollowerEntity) };
        }
    }
}
