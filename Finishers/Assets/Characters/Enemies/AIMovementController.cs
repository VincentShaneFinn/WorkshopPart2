using System;
using UnityEngine;

namespace Finisher.Characters
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (HumanCharacterController))] // TODO segregate enemy CharacterController into its own, since they will behave much differently
    public class AIMovementController : MonoBehaviour
    {

        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public HumanCharacterController character { get; private set; } // the character we are controlling
        public Transform target;                                    // target to aim for

        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<HumanCharacterController>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;
        }

        private void Update()
        {
            if (target != null)
                agent.SetDestination(target.position);

            if (agent.remainingDistance > agent.stoppingDistance)
                character.Move(agent.desiredVelocity, false);
            else
                character.Move(Vector3.zero, false);
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}
