﻿using System;
using System.Collections;
using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Enemies
{
    public enum ChaseSubState { Null, Direct, Arced, Surround }

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
        private Vector3 surroundTarget;
        private float range = 10f; //public surround range just for testing
        private float angle;
        private int sur_direction = 0;

        protected AICharacterController character;
        protected CharacterState characterState;
        protected SquadManager squadManager;
        protected Animator animator;
        protected AnimOverrideSetter animOverrideSetter;
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
            animator = GetComponent<Animator>();
            animOverrideSetter = GetComponent<AnimOverrideSetter>();
            combatSystem = GetComponent<CombatSystem>();

            if (squadManager)
            {
                characterState.DyingState.SubscribeToDeathEvent(removeFromSquad);
            }

            Physics.IgnoreLayerCollision(LayerNames.EnemyLayer, LayerNames.EnemyLayer, true);

            Vector3 targetDir = transform.position - combatTarget.transform.position;
            angle = Mathf.Atan2(targetDir.z, targetDir.x) * Mathf.Rad2Deg;
            surroundTarget = surroundPathfind();
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
            if (characterState.Dying ||
                playerState.IsDying ||
                characterState.Uninteruptable)
            {
                StopCurrentCoroutine();
            }
            else if (canChasePlayer())
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
                endEngagePlayerSequence();
            }
        }

        protected virtual void endEngagePlayerSequence()
        {
            character.RestoreStoppingDistance();
            character.RestoreMovementSpeedMultiplier();
            engagingPlayer = false;
            character.StopManualMovement();
        }

        protected virtual void pursuePlayer()
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
                float distan = Vector3.Distance(combatTarget.transform.position, this.transform.position);

                if (distan <= 6f)
                {
                    surroundMovement();
                }
            }
        }

        //do the calculation of the circle path around the player
        //return the position of next move
        private Vector3 surroundPathfind()
        {
            surroundTarget = combatTarget.transform.position;
            if (angle > 360)
            {
                angle -= 360;
            }
            float X, Z;
            X = range * Mathf.Cos(angle * Mathf.Deg2Rad);
            Z = range * Mathf.Sin(angle * Mathf.Deg2Rad);
            surroundTarget.x += X;
            surroundTarget.z += Z;

            return surroundTarget;
        }

        private void surroundMovement()
        {
            if (sur_direction == 0)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    sur_direction = -1;
                }
                else
                {
                    sur_direction = 1;
                }
            }
            Collider[] C = Physics.OverlapSphere(transform.position, 2.50f);
            foreach (Collider col in C)
            {
                if (col.tag.Equals("Enemy"))
                {
                    if (!col.transform.Equals(transform))
                    {   
                        Vector3 targetDir = transform.position - col.transform.position;
                        if ((Mathf.Atan2(targetDir.z, targetDir.x) * Mathf.Rad2Deg > 180 && sur_direction == 1)
                            ||(Mathf.Atan2(targetDir.z, targetDir.x) * Mathf.Rad2Deg < 180 && sur_direction == -1))
                        {
                            character.MovementSpeedMultiplier = .3f;
                        }
                        //this part have a problem
                        if (Vector3.Distance(col.transform.position, this.transform.position) < 1f)
                        {
                            sur_direction = sur_direction * -1;
                        }
                    }
                }

            }

            character.ManualyMoveCharacter(transform.right * sur_direction, strafing: true);
            character.LookAtTarget(combatTarget.transform);
            /*
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
            }*/

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

        private void removeFromSquad()
        {
            squadManager.RemoveEnemy(this.gameObject);
        }

        #endregion

    }
}
