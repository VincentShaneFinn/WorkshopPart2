using System;
using UnityEngine;

namespace Finisher.Characters
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    public class AICharacterController : FinisherCharacterController
    {

        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public FinisherCharacterController character { get; private set; } // the character we are controlling


        [Header("AI specific variables")]
        [SerializeField] public Transform target;                                    // target to aim for
        [HideInInspector] public bool canPerformNextAction = true;

        private bool agentCanMove = true; public bool CanAgentMove() { return agentCanMove; }
        private bool agentCanRotate = true; public bool CanAgentRotate() { return agentCanRotate; }

        private void Start()
        {
            Initialization();

            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<FinisherCharacterController>();

	        agent.updateRotation = false;
            agent.updatePosition = true;
        }

        private void Update()
        {
            // TODO build a system of allowing y movement for animations, like jump attack, or getting knocked into the air, which disables and re-enables the agent
            if (agent.isActiveAndEnabled)
            {
                if(target == null) { return; }

                agent.SetDestination(target.position);

                if (agent.remainingDistance > agent.stoppingDistance)
                    character.Move(agent.desiredVelocity, false);
                else
                {
                    character.Move(Vector3.zero, false);
                }
            }
            else
            {
                print("Agent is disabled");
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void TryAttack() // Consider puting this up from player and AI character controllers, and setting the animations from somewhere else
        {
            canPerformNextAction = false;
            if (UnityEngine.Random.Range(0,2) == 0)
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                animator.SetTrigger("Attack2");
            }

        }

        public void ToggleAgentMovement(bool AgentCanMove)
        {
            this.agentCanMove = AgentCanMove;
            if (this.agentCanMove)
            {
                agent.speed = 1;
            }
            else
            {
                agent.speed = Mathf.Epsilon; // must be > 0 to allow it to rotate for now
            }
        }

        public void ToggleAgentRotation(bool AgentCanRotate)
        {
            this.agentCanRotate = AgentCanRotate;
            if (this.agentCanRotate)
            {
                RestoreRotationLerp();
            }
            else
            {
                LockCharacterRotation();
            }
        }

        void Hit()
        {
            print("Enemy Hits");
        }

        // TODO again this is not great for letting some action rotate and others be prevented, consider a better way?
        public void ActionCompleted()
        {
            print("action completed");
            canPerformNextAction = true;
            ToggleAgentMovement(true);
            ToggleAgentRotation(true);
        }

        public void ActionBegun()
        {
            print("action started");
            ToggleAgentMovement(false);
            ToggleAgentRotation(false);
        }

        //void OnDrawGizmos()
        //{
        //    //draw sphere at target
        //    Gizmos.color = Color.black;
        //    Gizmos.DrawWireSphere(agent.destination, .3f);
        //}
    }
}
