using UnityEngine;

namespace BitBox.AI
{
    public class LookAtPlayerModule : Module
    {

        public float rotationSpeed;
        Transform playerTransform;
        Rigidbody myRigidbody;
        Transform myTransform;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);

            RunsInUpdate = false;
            RunsInFixedUpdate = true;

            /*IPlayer player = Container.GetUnique<IPlayer>();
            if (player != default) playerTransform = player.GetTransform();

            myRigidbody = agent.GetComponent<Rigidbody>();
            myTransform = agent.transform;*/
        }

        public override void UpdateExecutableState()
        {
            CanExecute = true;
        }

        public override void Execute()
        {
            base.Execute();
        }

        public override void FixedUpdate()
        {
            if (!IsExecuting) return;

            Vector3 direction = (playerTransform.position - myTransform.position);
            direction.y = 0;
            direction.Normalize();
            myRigidbody.rotation = Quaternion.Slerp(myRigidbody.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.fixedDeltaTime);
        }

        public override System.Type[] GetRequiredComponents()
        {
            return new System.Type[] { typeof(Rigidbody) };
        }
    }
}
