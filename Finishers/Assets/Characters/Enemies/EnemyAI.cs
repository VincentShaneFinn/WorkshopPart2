using System;
using System.Collections;
using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Enemies
{
    public enum ChaseSubState { Null, Direct, Arced, Surround }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(AICharacterController))]
    public class EnemyAI : MonoBehaviour
    {
        public bool ForcedSequenceRunning = false;

        private IEnumerator currentCoroutine;

        private bool engagingPlayer = false;
        private enum OOCState { Null, ReturningHome, Idle };
        private OOCState currentOOCState = OOCState.Null;
        public ChaseSubState currentChaseSubstate;

        [SerializeField] float chaseRadius = 5f;
        [SerializeField] protected float attackRadius = 1.5f;
        protected GameObject combatTarget = null;

        private Vector3 homeTargetPosition;
        private Quaternion homeTargetRotation;


        protected AICharacterController character;
        protected CharacterState characterState;
        private SquadManager squadManager;
        protected CombatSystem combatSystem;
        [SerializeField] CharacterStateSO playerState;

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

            if (squadManager)
            {
                characterState.DyingState.SubscribeToDeathEvent(removeFromSquad);
            }

            Physics.IgnoreLayerCollision(LayerNames.EnemyLayer, LayerNames.EnemyLayer, true);
        }

        void OnDestroy()
        {
            if (squadManager)
            {
                characterState.DyingState.UnsubscribeToDeathEvent(removeFromSquad);
                removeFromSquad();
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {

            if (canChasePlayer())
            {
                if (!engagingPlayer)
                {
                    startBehavior(engagePlayerSequence());
                }
            }
            else
            {
                outOfCombatSelector();
            }

        }

        private void startBehavior(IEnumerator coroutine)
        {
            StopCurrentCoroutine();
            currentCoroutine = coroutine;
            StartCoroutine(currentCoroutine);
        }

        protected void StopCurrentCoroutine()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine); //make a switch statement to stop gracefully\
                ((IDisposable)currentCoroutine).Dispose();
                currentCoroutine = null;
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
            ManagerState squadState = ManagerState.Waiting;
            if (squadManager)
            {
                squadState = squadManager.CurrentManagerState;
            }
            if(squadState == ManagerState.ReturnHome)
            {
                return false;
            }

            float distanceToPlayer = Vector3.Distance(combatTarget.transform.position, transform.position);
            if(distanceToPlayer <= chaseRadius || squadState == ManagerState.Attacking)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isPlayerInAttackRange(float customRadius = -1)
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

        #region State Behaviors

        #region Engage Player Sequence

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
                character.StopManualMovement();
            }
        }

        private void pursuePlayer()
        {
            character.SetTarget(combatTarget.transform);
            character.UseOptionalDestination = false;

            character.RestoreStoppingDistance();
            character.RestoreMovementSpeedMultiplier();
            character.StopManualMovement();

            if (currentChaseSubstate == ChaseSubState.Arced)
            {
                character.MovementSpeedMultiplier = .2f;
            }
            else if (currentChaseSubstate == ChaseSubState.Surround)
            {
                character.SetStoppingDistance(3.75f);
                surroundMovement();
            }
        }

        private void surroundMovement()
        {
            if (Input.GetKey(KeyCode.I))
            {
                character.ManualyMoveCharacter(transform.forward, strafing: true);
                character.LookAtTarget(combatTarget.transform);
            }
            else if (Input.GetKey(KeyCode.L))
            {
                character.ManualyMoveCharacter(transform.right, strafing: true);
                character.LookAtTarget(combatTarget.transform);
            }
            else if (Input.GetKey(KeyCode.K))
            {
                character.ManualyMoveCharacter(-transform.forward, strafing: true);
                character.LookAtTarget(combatTarget.transform);
            }
            else if (Input.GetKey(KeyCode.J))
            {
                character.ManualyMoveCharacter(-transform.right, strafing: true);
                character.LookAtTarget(combatTarget.transform);
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

        #endregion

        private void outOfCombatSelector()
        {
            if (!atHomePoint())
            {
                if (currentOOCState != OOCState.ReturningHome)
                {
                    startBehavior(returnHomeNode());
                }
            }
            else
            {
                if (currentOOCState != OOCState.Idle)
                {
                    startBehavior(idleStanceNode());
                }
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
            
        }

        private void stopByManager()
        {

        }

        private void removeFromSquad()
        {
            squadManager.RemoveEnemy(this.gameObject);
        }

        #endregion

    }
}
