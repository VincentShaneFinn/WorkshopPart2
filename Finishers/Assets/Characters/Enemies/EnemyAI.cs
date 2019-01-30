using UnityEngine;

using Finisher.Characters.Systems;
using System;

namespace Finisher.Characters.Enemies
{
    public enum EnemyState { Idle, ReturningHome, Patrolling, Chasing, Attacking }
    public enum ChaseSubState { Direct, Arced, Surround }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(AICharacterController))]
    public class EnemyAI : MonoBehaviour
    {

        public ChaseSubState currentChaseSubstate; //temporarily make us manually assign their chase method from inspector
        //TODO: this must be automated and set by the SquadManager

        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float attackRadius = 1.5f;
        [Tooltip("Will use player as the default Combat Target")]
        [SerializeField] GameObject combatTarget = null;
        [SerializeField] CharacterStateSO playerState;

        private AICharacterController character;
        private SquadManager squadManager;
        private CombatSystem combatSystem;
        private EnemyState currentState;
        private EnemyState directOrder;
        private Vector3 homeTargetPosition;
        private Quaternion homeTargetRotation;

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
            squadManager = GetComponentInParent<SquadManager>();
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

            if (playerState.state.IsInvulnerableSequence)
            {
                currentChaseSubstate = ChaseSubState.Surround;
            }
            else
            {
                currentChaseSubstate = ChaseSubState.Direct;
            }

            if (directOrder == EnemyState.ReturningHome)
            {
                state = EnemyState.ReturningHome;
            }
            else if (isPlayerInAttackRange() && !playerState.state.IsInvulnerableSequence)
            {
                state = EnemyState.Attacking;
            }
            else if (isPlayerInChaseRange() || directOrder == EnemyState.Chasing)
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
            //Only thing broken with this now is attack does not do the attack chains since no 
            //coroutine sequence is setup
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
                    pursuePlayer(); //TODO: a substate of chasing would be the 3 methods, DirectChase, ArcedChase, and Surround
                    break;
                case EnemyState.Attacking:
                    attackPlayer();
                    break;

            }
            
        }

        #region Helper Checkers

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

        #endregion

        #region State Behaviors

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

            character.RestoreStoppingDistance();
            character.RestoreMovementSpeedMultiplier();

            if (currentChaseSubstate == ChaseSubState.Arced)
            {
                character.MovementSpeedMultiplier = .2f;
            }
            else if(currentChaseSubstate == ChaseSubState.Surround)
            {
                character.SetStoppingDistance(3.5f);
            }
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

        #endregion

        #region Delegate Methods

        public void ChaseByManager()
        {
            directOrder = EnemyState.Chasing;
        }

        public void StopByManager()
        {
            directOrder = EnemyState.ReturningHome;
        }

        #endregion

    }
}
