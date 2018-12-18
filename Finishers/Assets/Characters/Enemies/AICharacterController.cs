using System;
using UnityEngine;

namespace Finisher.Characters
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    public class AICharacterController : CharacterAnimator
    {

        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding

        [Header("AI specific variables")]
        [SerializeField] public Transform target;                                    // target to aim for

        private bool agentCanMove = true; public bool CanAgentMove() { return agentCanMove; }
        private bool agentCanRotate = true; public bool CanAgentRotate() { return agentCanRotate; }

        void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();

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
                    Move(agent.desiredVelocity, false);
                else
                {
                    Move(Vector3.zero, false);
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

        //void OnDrawGizmos()
        //{
        //    //draw sphere at target
        //    Gizmos.color = Color.black;
        //    Gizmos.DrawWireSphere(agent.destination, .3f);
        //}
    }
}
