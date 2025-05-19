using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace BitBox.AI
{
    [Serializable]
    public abstract class Module
    {
        protected Agent _agent;

        [ReadOnly, ShowInInspector, FoldoutGroup("Information", true)] public bool CanExecute { get; protected set; }
        [ReadOnly, ShowInInspector, FoldoutGroup("Information", true)] public bool IsExecuting { get; protected set; }
        
        [HideInInspector] public bool RunsInUpdate;
        [HideInInspector] public bool RunsInFixedUpdate;
        
        [FoldoutGroup("Animation", true), SerializeField] protected bool _hasAnimation;
        [FoldoutGroup("Animation", true), ShowIf(nameof(_hasAnimation)), SerializeField] protected string _animationName;
        
        protected Animator _animator;
        
        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
            if (_hasAnimation) _animator = _agent.GetComponent<Animator>();
        }

        public abstract void UpdateExecutableState();

        public virtual void Execute()
        {
            IsExecuting = true;
            if (_hasAnimation) _animator.CrossFadeInFixedTime(_animationName, .1f);
        }

        public virtual void Update()
        {

        }

        public virtual void FixedUpdate()
        {
            
        }

        public virtual void Stop() 
        {
            IsExecuting = false;
        }

        public virtual void DrawGizmos()
        {
            // This is where gizmos information is added for the module
        }

        public virtual Type[] GetRequiredComponents()
        {
            return Type.EmptyTypes;
        }
    }
}