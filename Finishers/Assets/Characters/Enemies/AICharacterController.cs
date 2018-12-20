using System;
using UnityEngine;

namespace Finisher.Characters
{
    public class AICharacterController : CharacterAnimator
    {

        private UnityEngine.AI.NavMeshAgent agent; // the navmesh agent required for the path finding
        private Transform target; // target to aim for

        [Header("AI Specific Attributes")]
        [SerializeField] float baseOffset = -0.04f;
        [SerializeField] float stoppingDistance = 1.3f;

        #region CanMove and CanRotate Overrides
        public override bool CanMove
        {
            get
            {
                return base.CanMove;
            }
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
            get
            {
                return base.CanRotate;
            }
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

        void Start()
        {
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
                if(Dying) {
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
                // TODO build a system of allowing y movement for animations, like jump attack, or getting knocked into the air, which disables and re-enables the agent
                if (target)
                {
                    agent.SetDestination(target.position);

                    if (agent.remainingDistance > agent.stoppingDistance && CanMove)
                    {
                        MoveCharacter(agent.desiredVelocity, false);
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
                // todo get a turn amount to look at target and use that instead, mimic the auto cam  also only change the y rotation
                transform.LookAt(target);
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        //void OnDrawGizmos()
        //{
        //    //draw sphere at target
        //    Gizmos.color = Color.black;
        //    Gizmos.DrawWireSphere(agent.destination, .3f);
        //}

    }
}
