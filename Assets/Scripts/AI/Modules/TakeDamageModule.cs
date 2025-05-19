using UnityEngine;

namespace BitBox.AI
{
    [CreateAssetMenu(fileName = "Take Damage Module", menuName = "AI Modules/Take Damage", order = 1)]
    public class TakeDamageModule : Module
    {
        
        public virtual int x { get; }
        /*public LayerMask damageLayers;
        public float delayBetweenHits = 0.2f;
        float lastTriggerTime = -100f;
        float lastExecutionTime = -100f;

        ICollidable collidable;
        IKillable killable;
        IBlinkable blinkable;
        int damageFromLastAttack;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);

            runsInUpdate = true;
            runsInFixedUpdate = false;

            collidable = Container.Get<ICollidable>(agent);
            blinkable = Container.Get<IBlinkable>(agent);
            killable = Container.Get<IKillable>(agent);

            if(collidable == null) Debug.Log($"No {typeof(ICollidable)} found on {agent.gameObject.name}");
            else Debug.Log($"Found {typeof(ICollidable)} on {agent.gameObject.name}");
            
            collidable.TriggerEnter += TakeDamage;
        }

        public override bool CanExecute()
        {
            canExecute = !isExecuting && Time.time < lastTriggerTime + delayBetweenHits;
            return canExecute;
        }

        public void TakeDamage(int damage)
        {
            lastTriggerTime = Time.time;
            damageFromLastAttack = damage;
        }

        public override void Execute()
        {
            base.Execute();
            lastExecutionTime = Time.time;
            killable.TakeDamage(damageFromLastAttack);
            if (blinkable != null && killable.GetHealth() > 0) blinkable.Blink();
        }

        public override void Update()
        {
            if (!isExecuting) return;
            if (Time.time > lastExecutionTime + delayBetweenHits)
            {
                Interrupt();
            }
        }

        public override System.Type[] GetRequiredComponents()
        {
            return new System.Type[] { typeof(Collider) };
        }*/
        public override void UpdateExecutableState()
        {
            CanExecute = true;
        }
    }
}
