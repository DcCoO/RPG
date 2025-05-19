using Pathfinding;
using UnityEngine;

namespace BitBox.AI
{
    [CreateAssetMenu(fileName = "Retreat Module", menuName = "AI Modules/Retreat", order = 1)]
    public class RetreatModule : Module
    {
        public float desiredDistanceFromTarget;
        public float followSpeed;

        Seeker seeker;

        Transform myTransform;
        Rigidbody myRigidbody;
        Transform playerTransform;

        float timeToRepath;
        float lastRepathTime;
        Path path;
        int currentWaypoint;

        const float distanceToCompare = 0.3f;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);

            RunsInUpdate = false;
            RunsInFixedUpdate = true;

            myTransform = agent.transform;
            myRigidbody = agent.GetComponent<Rigidbody>();
            /*IPlayer player = Container.GetUnique<IPlayer>();
            if (player != default) playerTransform = player.GetTransform();
            seeker = agent.GetComponent<Seeker>();


            timeToRepath = 0.4f;
            lastRepathTime = float.NegativeInfinity;*/
        }

        public override void UpdateExecutableState()
        {
            if (IsExecuting)
            {
                CanExecute = false;
            }
            /*if (playerTransform == null)
            {
                IPlayer player = Container.GetUnique<IPlayer>();
                if (player != default) playerTransform = player.GetTransform();
                else return false;
            }*/

            CanExecute = Vector3.Distance(playerTransform.position, myTransform.position) < desiredDistanceFromTarget;
        }

        public override void Execute()
        {
            base.Execute();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!IsExecuting) return;

            

            if (Time.time > lastRepathTime + timeToRepath /*&& seeker.IsDone()*/)
            {
                lastRepathTime = Time.time;

                Vector3 destination = playerTransform.position + (myTransform.position - playerTransform.position).normalized * (desiredDistanceFromTarget + 2 * distanceToCompare);

                // Start a new path to the targetPosition, call the the OnPathComplete function
                // when the path has been calculated (which may take a few frames depending on the complexity)
                //seeker.StartPath(myTransform.position, destination, OnPathComplete);
            }

            if (path == null) return;

            // Check in a loop if we are close enough to the current waypoint to switch to the next one.
            // We do this in a loop because many waypoints might be close to each other and we may reach
            // several of them in the same frame.
            bool reachedEndOfPath = false;
            // The distance to the next waypoint in the path
            float distanceToWaypoint;

            while (true)
            {
                // If you want maximum performance you can check the squared distance instead to get rid of a
                // square root calculation. But that is outside the scope of this tutorial.
                distanceToWaypoint = Vector3.SqrMagnitude(myTransform.position - path.vectorPath[currentWaypoint]);
                
                if (distanceToWaypoint < distanceToCompare)
                {
                    // Check if there is another waypoint or if we have reached the end of the path
                    if (currentWaypoint + 1 < path.vectorPath.Count)
                    {
                        currentWaypoint++;
                    }
                    else
                    {
                        // Set a status variable to indicate that the agent has reached the end of the path.
                        // You can use this to trigger some special code if your game requires that.
                        reachedEndOfPath = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            //var moveDirection = (myTransform.position - path.vectorPath[currentWaypoint]).XZ().normalized;

            //var projectedPlayer = playerTransform.position;
            //projectedPlayer.y = myTransform.position.y;
            //myRigidbody.rotation = Quaternion.Slerp(myRigidbody.rotation, Quaternion.LookRotation(projectedPlayer - myTransform.position), 10 * Time.fixedDeltaTime);
            //myRigidbody.rotation = Quaternion.Slerp(myRigidbody.rotation, Quaternion.LookRotation(myTransform.position - path.vectorPath[currentWaypoint]), 10 * Time.fixedDeltaTime);

            /*myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(moveDirection), 10 * Time.fixedDeltaTime);
            //myTransform.LookAt(myTransform.position + playerDirection, Vector3.up);
            if (reachedEndOfPath)
            {
                Interrupt();
                return;
            }



            Vector3 direction = (path.vectorPath[currentWaypoint] - myTransform.position);
            direction.y = 0;
            direction.Normalize();
            myRigidbody.velocity = direction * followSpeed;
            Debug.DrawRay(myTransform.position + Vector3.up * 0.4f, direction, Color.red);*/
        }

        public override void Stop()
        {
            base.Stop();
            currentWaypoint = 0;
            myRigidbody.linearVelocity = Vector3.zero;
            if (!myRigidbody.isKinematic) myRigidbody.angularVelocity = Vector3.zero;
            path = null;
        }

        public void OnPathComplete(Path p)
        {
            p.Claim(this);

            if (p.error)
            {
                p.Release(this);
            }
            else
            {
                if (path != null) path.Release(this);
                path = p;
                currentWaypoint = 0;
            }
        }

        public override System.Type[] GetRequiredComponents()
        {
            return new System.Type[] { typeof(Seeker) };
        }
    }
}
