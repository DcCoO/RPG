using UnityEngine;

namespace BitBox.AI
{
    public class RangedAttackModule : Module
    {
        /*public int attacksBeforeReload;
        public float attackDuration;
        public float delayBetweenAttacks;
        public float reloadDuration;

        public float maxAngleDifference;
        public Vector2 distanceRange;

        int attacksLeft;
        float lastAttackTime = -100f;

        Transform playerTransform;
        Transform myTransform;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);

            runsInUpdate = false;
            runsInFixedUpdate = true;

            IPlayer player = Container.GetUnique<IPlayer>();
            if (player != default) playerTransform = player.GetTransform();

            myTransform = agent.transform;
            attacksLeft = attacksBeforeReload;
        }

        public override bool CanExecute()
        {
            var angle = Vector3.Angle(myTransform.forward.XZ(), (playerTransform.position - myTransform.position).XZ());
            var distance = Vector3.Distance(myTransform.position.XZ(), playerTransform.position.XZ());

            if (isExecuting) return canExecute = false;

            if (attacksLeft <= 0)
            {
                if (Time.time < lastAttackTime + attackDuration + reloadDuration) return canExecute = false;
                else attacksLeft = attacksBeforeReload;
            }
            else
            {
                if (Time.time < lastAttackTime + attackDuration + delayBetweenAttacks) return canExecute = false;
            }

            if (angle > maxAngleDifference) return canExecute = false;
            return canExecute = (distanceRange.x <= distance && distance <= distanceRange.y);
        }

        public override void Execute()
        {
            base.Execute();
            lastAttackTime = Time.time;
            --attacksLeft;
        }

        public override void FixedUpdate()
        {
            if (!isExecuting) return;
            if (Time.time > lastAttackTime + attackDuration)
            {
                Interrupt();
            }
        }*/
        public override void UpdateExecutableState()
        {
            //return true;
        }
    }
}
