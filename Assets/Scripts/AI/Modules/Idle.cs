using System;
using UnityEngine;

namespace BitBox.AI
{
    public class Idle : Module
    {
        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            RunsInUpdate = false;
            RunsInFixedUpdate = false;
        }

        public override void UpdateExecutableState()
        {
            CanExecute = true;
        }

        public override void Execute()
        {
            if (IsExecuting) return;
            base.Execute();
        }

        public override Type[] GetRequiredComponents()
        {
            if (_hasAnimation) return new Type[] { typeof(Animator) };
            return Type.EmptyTypes;
        }
    }
}