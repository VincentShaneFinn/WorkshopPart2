using UnityEngine;

namespace Finisher.Characters.Enemies
{
    public class AICharacterController : CharacterAnimator
    {
        public bool UseOptionalDestination = false;
        public Vector3 OptionalDestination = Vector3.zero;

        private Transform target; // target to aim for

        [Header("AI Specific Attributes")]
        [SerializeField] float baseOffset = -0.04f;
        [SerializeField] float stoppingDistance = 1.3f;

        #region CanMove and CanRotate Overrides

        public override bool CanMove
        {
            get { return base.CanMove; }
            set
            {
                base.CanMove = value;
                if (base.CanMove)
                {
                    agent.speed = 1;
                }
                else
                {
                    agent.speed = Mathf.Epsilon; // must be > 0 to allow it to rotate for now
                }
            }
        }

        public override bool CanRotate
        {
            get { return base.CanRotate; }
            set
            {
                base.CanRotate = value;
                if (this.CanRotate)
                {
                    RestoreRotationLerp();
                }
                else
                {
                    LockCharacterRotation();
                }
            }
        }

        #endregion

        private UnityEngine.AI.NavMeshAgent agent; // the navmesh agent required for the path finding

        protected override void Start()
        {
            base.Start();

            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            //agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            agent.baseOffset = baseOffset;
            agent.stoppingDistance = stoppingDistance;
            agent.speed = 1;
	        agent.updateRotation = false;
            agent.updatePosition = true;

            CurrentLookTarget = transform;
        }

        private void Update()
        {
            if (agent.isActiveAndEnabled)
            {
                if(characterState.Dying) {
                    agent.SetDestination(transform.position);
                    return;
                }
                 AttemptMoveToTarget();
            }
            else
            {
                MoveCharacter(Vector3.zero); // todo allow movement when the agent is inactive via another control method
            }
        }

        private void AttemptMoveToTarget()
        {
            if (CanMove || CanRotate)
            {
                if (target)
                {
                    setAgentDestination();

                    if (agent.remainingDistance > agent.stoppingDistance && CanMove)
                    {
                        MoveCharacter(agent.desiredVelocity);
                    }
                    else
                    {
                        StationaryLookAt();
                    }
                }
            }
            else
            {
                agent.SetDestination(transform.position);
                MoveCharacter(Vector3.zero);
            }
            
        }

        private void StationaryLookAt()
        {
            MoveCharacter(Vector3.zero);
            if (CanRotate)
            {
                if (target)
                {
                    transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                }
            }
        }



        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        private void setAgentDestination()
        {
            if (UseOptionalDestination)
            {
                agent.SetDestination(OptionalDestination);
                agent.stoppingDistance = .25f;
            }
            else if (target)
            {
                agent.SetDestination(target.position);
                agent.stoppingDistance = stoppingDistance;
            }
        }


        //void OnDrawGizmos()
        //{
        //    //draw sphere at target
        //    Gizmos.color = Color.black;
        //    Gizmos.DrawWireSphere(agent.destination, .3f);
        //}
    }
}
