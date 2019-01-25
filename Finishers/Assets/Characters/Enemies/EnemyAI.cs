using UnityEngine;

using Finisher.Characters.Systems;
using System;

namespace Finisher.Characters.Enemies
{
    public enum EnemyState { Idle, ReturningHome, Patrolling, Chasing, Attacking }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(AICharacterController))]
    public class EnemyAI : MonoBehaviour
    {

        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float attackRadius = 1.5f;
        [Tooltip("Will use player as the default Combat Target")]
        [SerializeField] GameObject combatTarget = null;
        [SerializeField] CharacterStateSO playerState;

        private AICharacterController character;
        private SquadeManager squadManager;
        private CombatSystem combatSystem;
        private EnemyState currentState;
        private EnemyState directOrder;
        private Vector3 homeTargetPosition;
        private Quaternion homeTargetRotation;
        //TODO: theres a simpler way to handle the order here

        // Use this for initialization
        void Start()
        {
            homeTargetPosition = transform.position;
            homeTargetRotation = transform.rotation;
            if (combatTarget == null)
            {
                combatTarget = GameObject.FindGameObjectWithTag("Player");
            }
            character = GetComponent<AICharacterController>();
            squadManager = GetComponentInParent<SquadeManager>();
            combatSystem = GetComponent<CombatSystem>();
            currentState = EnemyState.Idle;
            directOrder = EnemyState.Idle;

            if (squadManager)
            {
                squadManager.OnEnemiesEngage += ChaseByManager;
                squadManager.OnEnemiesDisengage += StopByManager;
            }
        }

        void OnDestroy()
        {
            if (squadManager)
            {
                squadManager.OnEnemiesEngage -= ChaseByManager;
                squadManager.OnEnemiesDisengage -= StopByManager;
            }
        }

        // Update is called once per frame
        void Update()
        {

            EnemyState state;

            if(directOrder == EnemyState.Chasing) //todo whats a better way than these 2 ifs
            {
                state = EnemyState.Chasing;
            }
            else if (directOrder == EnemyState.ReturningHome)
            {
                state = EnemyState.ReturningHome;
            }
            else if (isPlayerInAttackRange())
            {
                state = EnemyState.Attacking;
            }
            else if (isPlayerInChaseRange())
            {
                state = EnemyState.Chasing;
            }
            else if (!atHomePoint())
            {
                state = EnemyState.ReturningHome;
            }
            else
            {
                state = EnemyState.Idle;
            }

            //if (state != currentState) //Only do stuff if it changes
            {
                currentState = state;
            }

            switch (currentState)
            {
                case EnemyState.Idle:
                    idleStance();
                    break;
                case EnemyState.Patrolling:
                    throw new NotImplementedException();
                case EnemyState.ReturningHome:
                    returnHome();
                    break;
                case EnemyState.Chasing:
                    pursuePlayer();
                    break;
                case EnemyState.Attacking:
                    attackPlayer();
                    break;

            }
            
        }

        private bool atHomePoint()
        {
            float distanceToHome = Vector3.Distance(transform.position, homeTargetPosition);
            if (distanceToHome <= .2f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isPlayerInChaseRange()
        {
            float distanceToPlayer = Vector3.Distance(combatTarget.transform.position, transform.position);
            if(distanceToPlayer <= chaseRadius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool isPlayerInAttackRange()
        {
            float distanceToPlayer = Vector3.Distance(combatTarget.transform.position, transform.position);
            if (distanceToPlayer <= attackRadius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void idleStance()
        {
            character.SetTarget(transform);
            character.UseOptionalDestination = false;
            transform.rotation = homeTargetRotation;
        }

        private void pursuePlayer()
        {
            character.SetTarget(combatTarget.transform);
            character.UseOptionalDestination = false;
        }

        private void attackPlayer()
        {
            if (squadManager && squadManager.CurrentManagerState == ManagerState.Waiting)
            {
                //CURRENTLY MAJORLY BUGGED WHERE IT CRASHES BUILDS ONLY
                squadManager.SendWakeUpCallToEnemies();
            }

            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                combatSystem.HeavyAttack();
            }
            else
            {
                combatSystem.LightAttack();
            }

        }

        private void returnHome()
        {
            character.SetTarget(transform);
            character.OptionalDestination = homeTargetPosition;
            character.UseOptionalDestination = true;
            if (atHomePoint())
            {
                directOrder = EnemyState.Idle;
            }
        }

        public void ChaseByManager()
        {
            directOrder = EnemyState.Chasing;
        }

        public void StopByManager()
        {
            directOrder = EnemyState.ReturningHome;
        }
    }
}
