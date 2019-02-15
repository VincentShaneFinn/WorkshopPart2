using System;
using System.Collections;
using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Enemies
{
    public enum EnemyState { Null, EngagePlayer, OutOfCombat }
    public enum ChaseSubState { Null, Direct, Arced, Surround }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(AICharacterController))]
    public class EnemyAI : MonoBehaviour
    {

        private bool engagingPlayer = false;
        private enum OOCState { Null, ReturningHome, Idle };
        private OOCState currentOOCState = OOCState.Null;

        public ChaseSubState currentChaseSubstate; //temporarily make us manually assign their chase method from inspector
        //TODO: this must be automated and set by the SquadManager

        [SerializeField] float chaseRadius = 5f;
        [SerializeField] protected float attackRadius = 1.5f;
        [SerializeField] CharacterStateSO playerState;

        protected GameObject combatTarget = null;

        protected AICharacterController character;
        protected CharacterState characterState;
        private SquadManager squadManager;
        protected CombatSystem combatSystem;
        protected EnemyState currentState;
        private EnemyState directOrder;
        private Vector3 homeTargetPosition;
        private Quaternion homeTargetRotation;

        private IEnumerator currentCoroutine;

        // Use this for initialization
        protected virtual void Start()
        {
            homeTargetPosition = transform.position;
            homeTargetRotation = transform.rotation;
            if (combatTarget == null)
            {
                combatTarget = GameObject.FindGameObjectWithTag("Player");
            }
            character = GetComponent<AICharacterController>();
            characterState = GetComponent<CharacterState>();
            squadManager = GetComponentInParent<SquadManager>();
            combatSystem = GetComponent<CombatSystem>();
            currentState = EnemyState.Null;
            directOrder = EnemyState.Null;     

            if (squadManager)
            {
                characterState.DyingState.SubscribeToDeathEvent(removeFromSquad);
                squadManager.OnEnemiesEngage += chaseByManager;
                squadManager.OnEnemiesDisengage += stopByManager;
            }
        }

        void OnDestroy()
        {
            if (squadManager)
            {
                characterState.DyingState.UnsubscribeToDeathEvent(removeFromSquad);
                removeFromSquad();
                squadManager.OnEnemiesEngage -= chaseByManager;
                squadManager.OnEnemiesDisengage -= stopByManager;
            }
        }

        // Update is called once per frame
        void Update()
        {

            #region Manual Input
            //Test Manual Movement
            //var straffing = true;

            ////   I
            //// J K L to move all enemies 
            //if (Input.GetKey(KeyCode.I))
            //{
            //    character.ManualyMoveCharacter(transform.forward, straffing);
            //    character.LookAtTarget(combatTarget.transform);
            //}
            //else if (Input.GetKey(KeyCode.L))
            //{
            //    character.ManualyMoveCharacter(transform.right, straffing);
            //    character.LookAtTarget(combatTarget.transform);
            //}
            //else if (Input.GetKey(KeyCode.K))
            //{
            //    character.ManualyMoveCharacter(-transform.forward, straffing);
            //    character.LookAtTarget(combatTarget.transform);
            //}
            //else if (Input.GetKey(KeyCode.J))
            //{
            //    character.ManualyMoveCharacter(-transform.right, straffing);
            //    character.LookAtTarget(combatTarget.transform);
            //}
            //else
            //{
            //    character.StopManualMovement();
            //}
            //NOTE: right now you must let the character know you have stopped manually controlling it
            //TODO: refactor so it optionally uses input a little better
            //TODO: LookAtTarget should be centralized so we can guarantee only one thing is setting it, but right now its good
            //Done Testing
            #endregion

            #region Old State Machine
            //if (!combatTarget)
            //{
            //    return;
            //}

            //EnemyState state;

            //if (playerState.IsFinishing || playerState.IsGrabbing)
            //{
            //    currentChaseSubstate = ChaseSubState.Surround;
            //}
            //else if (!squadManager)
            //{
            //    currentChaseSubstate = ChaseSubState.Null;
            //}

            //if (directOrder == EnemyState.ReturningHome)
            //{
            //    state = EnemyState.ReturningHome;
            //}
            //else if (isPlayerInAttackRange() && !(playerState.IsFinishing || playerState.IsGrabbing))
            //{
            //    state = EnemyState.Attacking;
            //}
            //else if (isPlayerInChaseRange() || directOrder == EnemyState.Chasing)
            //{
            //    state = EnemyState.Chasing;
            //}
            //else if (!atHomePoint())
            //{
            //    state = EnemyState.ReturningHome;
            //}
            //else
            //{
            //    state = EnemyState.Idle;
            //}

            ////if (state != currentState) //Only do stuff if it changes
            ////Only thing broken with this now is attack does not do the attack chains since no 
            ////coroutine sequence is setup
            //{
            //    currentState = state;
            //}

            //makeAttackDecision();

            //switch (currentState)
            //{
            //    case EnemyState.Idle:
            //        idleStance();
            //        break;
            //    case EnemyState.Patrolling:
            //        throw new NotImplementedException();
            //    case EnemyState.ReturningHome:
            //        returnHome();
            //        break;
            //    case EnemyState.Chasing:
            //        pursuePlayer(); //TODO: a substate of chasing would be the 3 methods, DirectChase, ArcedChase, and Surround
            //        break;
            //    case EnemyState.Attacking:
            //        if (squadManager && squadManager.CurrentManagerState == ManagerState.Waiting)
            //        {
            //            //CURRENTLY MAJORLY BUGGED WHERE IT CRASHES BUILDS ONLY
            //            squadManager.SendWakeUpCallToEnemies();
            //        }
            //        attackPlayer();
            //        break;

            //}
            #endregion

            if (canChasePlayer())
            {
                if (!engagingPlayer)
                {
                    StartBehavior(engagePlayerSequence());
                }
            }
            else
            {
                outOfCombatSelector();
            }

        }

        private void StartBehavior(IEnumerator coroutine)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine); //make a switch statement to stop gracefully\
                ((IDisposable)currentCoroutine).Dispose();   
            }
            currentCoroutine = coroutine;
            StartCoroutine(currentCoroutine);
            
        }

        private void outOfCombatSelector()
        {
            //return home sequence
            if (!atHomePoint())
            {
                if (currentOOCState != OOCState.ReturningHome)
                {
                    StartBehavior(returnHomeNode());
                }
            }
            else
            {
                if (currentOOCState != OOCState.Idle)
                {
                    StartBehavior(idleStanceNode());
                }
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

        private bool canChasePlayer()
        {
            if(directOrder == EnemyState.OutOfCombat)
            {
                return false;
            }

            float distanceToPlayer = Vector3.Distance(combatTarget.transform.position, transform.position);
            if(distanceToPlayer <= chaseRadius || directOrder == EnemyState.EngagePlayer)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool isPlayerInAttackRange(float customRadius = -1)
        {
            float distanceToPlayer = Vector3.Distance(combatTarget.transform.position, transform.position);

            float radius = attackRadius;
            if (customRadius >= 0)
            {
                radius = customRadius;
            }

            if (distanceToPlayer <= radius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        protected virtual void makeAttackDecision()
        {
            
        }

        #region State Behaviors

        IEnumerator engagePlayerSequence()
        {
            engagingPlayer = true;

            try
            {
                while (!isPlayerInAttackRange())
                {
                    pursuePlayer();
                    yield return null;
                }
                attackPlayer();
            }
            finally
            {
                character.RestoreStoppingDistance();
                character.RestoreMovementSpeedMultiplier();
                engagingPlayer = false;
            }
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
            else if (currentChaseSubstate == ChaseSubState.Surround)
            {
                character.SetStoppingDistance(3.75f);
            }
        }

        protected virtual void attackPlayer()
        {
            if (squadManager && squadManager.CurrentManagerState == ManagerState.Waiting)
            {
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

        IEnumerator returnHomeNode()
        {
            currentOOCState = OOCState.ReturningHome;

            try
            {
                character.SetTarget(transform);
                character.OptionalDestination = homeTargetPosition;
                character.UseOptionalDestination = true;
                character.RestoreStoppingDistance();
                character.RestoreMovementSpeedMultiplier();
                yield return new WaitUntil(() => atHomePoint());
                yield return null; //FrameSpacer
            }
            finally
            {
                currentOOCState = OOCState.Null;
            }
        }

        IEnumerator idleStanceNode()
        {
            currentOOCState = OOCState.Idle;

            try
            {
                if (transform.rotation != homeTargetRotation)
                {
                    character.SetTarget(transform);
                    character.UseOptionalDestination = false;
                    transform.rotation = homeTargetRotation;
                    directOrder = EnemyState.Null;
                }
                yield return null;
            }
            finally
            {
                currentOOCState = OOCState.Null;
            }
        }

        #endregion

        #region Delegate Methods

        private void chaseByManager()
        {
            directOrder = EnemyState.EngagePlayer;
        }

        private void stopByManager()
        {
            directOrder = EnemyState.OutOfCombat;
        }

        private void removeFromSquad()
        {
            squadManager.RemoveEnemy(this.gameObject);
        }

        #endregion

    }
}
