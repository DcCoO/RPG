using System;
using UnityEngine;

namespace BitBox.Gameplay
{
    /// <summary>
    /// This MonoBehaviour is used to identify entities that can be target of the Target Lock system.
    /// It also contains an offset that can be used to adjust the position of the target lock indicator.
    /// </summary>
    public class LockableEntity : Entity
    {
        [SerializeField] private Vector3 _offset;
        private Transform _transform;
        public Vector3 Position => transform.position;
        public Vector3 LockPosition => transform.position + _offset;

        private void Start()
        {
            _transform = transform;
        }
    }
}