﻿using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Enemies
{
    public enum EnemyState { idle, Patrolling, Chasing, Attacking }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(AICharacterController))]
    [RequireComponent(typeof(SquadeManager))]

    public class EnemyAI : MonoBehaviour
    {

        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float attackRadius = 1.5f;
        [Tooltip("Will use player as the default Combat Target")]
        [SerializeField] GameObject combatTarget = null;
        [SerializeField] CharacterStateSO playerState;

        AICharacterController character;
        public GameObject manager;
        SquadeManager Manager;
        private CombatSystem combatSystem;
        public EnemyState state;
        private Vector3 homeTargetPosition;
        private Quaternion homeTargetRotation;
        private bool outofhome = false;
        private bool attackorder = false;


        // Use this for initialization
        void Start()
        {
            homeTargetPosition = transform.position;
            homeTargetRotation = transform.rotation;
            if (combatTarget == null)
            {
                combatTarget = GameObject.FindGameObjectWithTag("Player");
            }
            if (manager == null)
            {
                manager = GameObject.FindGameObjectWithTag("EnemyManager");
            }
            character = GetComponent<AICharacterController>();
            Manager = manager.GetComponent<SquadeManager>();
            combatSystem = GetComponent<CombatSystem>();
            state = EnemyState.idle;

        }

        // Update is called once per frame
        void Update()
        {
            // todo make a state machine
            pursueNearbyPlayer();
            if (!playerState.DyingState.Dying)
            {
                attackPlayerIfNear();
            }    
        }

        private void pursueNearbyPlayer()
        {
            float distanceToPlayer = Vector3.Distance(combatTarget.transform.position, transform.position);
            if (attackorder)
            {
                Application.Quit();
            }
            else if (distanceToPlayer <= chaseRadius && !outofhome)
            {
                character.SetTarget(combatTarget.transform);
                character.UseOptionalDestination = false;
                state = EnemyState.Chasing;
                Manager.Startattack();
            }
            else
            {
                character.SetTarget(transform);
                character.OptionalDestination = homeTargetPosition;
                character.UseOptionalDestination = true;
                if(Vector3.Distance(transform.position, homeTargetPosition) <= .26f && transform.rotation != homeTargetRotation)
                {
                    transform.rotation = homeTargetRotation;
                    outofhome = false;
                }
                state = EnemyState.idle;
            }
        }

        private void attackPlayerIfNear()
        {
            float distanceToPlayer = Vector3.Distance(combatTarget.transform.position, transform.position);
            if (distanceToPlayer <= attackRadius)
            {
                state = EnemyState.Attacking;
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    combatSystem.HeavyAttack();
                }
                else
                {
                    combatSystem.LightAttack();
                }
            }
        }

        public void StopByManager()
        {
            character.SetTarget(transform);
            character.OptionalDestination = homeTargetPosition;
            character.UseOptionalDestination = true;
            outofhome = true;
            attackorder = false;
        }

        public void AttackByManager()
        {
            character.SetTarget(combatTarget.transform);
            character.UseOptionalDestination = false;
            state = EnemyState.Chasing;
            attackorder = true;
        }
    }
}
