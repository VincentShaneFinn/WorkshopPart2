using System.Collections;
using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters
{
    public enum EnemyState { idle, Patrolling, Chasing, Attacking }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(AICharacterController))]
    public class EnemyAI : MonoBehaviour
    {

        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float attackRadius = 1.5f;
        [Tooltip("Will use player as the default Combat Target")]
        [SerializeField] GameObject combatTarget = null;
        [SerializeField] CharacterStateSO playerState;

        AICharacterController character;
        private CombatSystem combatSystem;

        private EnemyState currentState;


        // Use this for initialization
        void Start()
        {
            if (combatTarget == null)
            {
                combatTarget = GameObject.FindGameObjectWithTag("Player");
            }
            character = GetComponent<AICharacterController>();
            combatSystem = GetComponent<CombatSystem>();
        }

        // Update is called once per frame
        void Update()
        {

            // todo make a state machine
            pursueNearbyPlayer();
            //if (currentState != EnemyState.Attacking && combatSystem.isActiveAndEnabled) // should be in range, then start attacking if we arent already
            //{
            //     attackPlayerIfNear();
            //}
            if (!playerState.DyingState.Dying)
            {
                attackPlayerIfNear();
            }    
        }

        private void pursueNearbyPlayer()
        {
            float distanceToPlayer = Vector3.Distance(combatTarget.transform.position, transform.position);
            if (distanceToPlayer <= chaseRadius)
            {
                character.SetTarget(combatTarget.transform);
            }
            else
            {
                character.SetTarget(transform);
            }
        }

        private void attackPlayerIfNear()
        {
            float distanceToPlayer = Vector3.Distance(combatTarget.transform.position, transform.position);
            if (distanceToPlayer <= attackRadius)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    combatSystem.HeavyAttack();
                }
                else
                {
                    combatSystem.LightAttack();
                }
                currentState = EnemyState.Attacking;
            }
        }
    }
}
