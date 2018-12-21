using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    public enum EnemyState { idle, Patrolling, Chasing, Attacking }

    [RequireComponent(typeof(AICharacterController))]
    public class EnemyAI : MonoBehaviour
    {

        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float attackRadius = 1.5f;

        AICharacterController aiCharacter;
        private CombatSystem combatSystem;
        private EnemyState currentState;
        GameObject player = null;

        // Use this for initialization
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            aiCharacter = GetComponent<AICharacterController>();
            combatSystem = GetComponent<CombatSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            //if (player == null) { return; }

            TestInput();
            // todo make a state machine
            pursueNearbyPlayer();
            if (aiCharacter.CanAct && currentState != EnemyState.Attacking) // should be in range, then start attacking if we arent already
            {
                attackPlayerIfNear();
            }
        }

        private void TestInput()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                aiCharacter.CanMove = false;
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                aiCharacter.CanMove = true;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                aiCharacter.CanRotate = false;
            }
            if (Input.GetKeyUp(KeyCode.G))
            {
                aiCharacter.CanRotate = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                aiCharacter.Kill();
            }
        }
        private void pursueNearbyPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= chaseRadius)
            {
                aiCharacter.SetTarget(player.transform);
            }
            else
            {
                aiCharacter.SetTarget(transform);
            }
        }

        private void attackPlayerIfNear()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= attackRadius)
            {
                print(Time.time);
                combatSystem.LightAttack();
                currentState = EnemyState.Attacking;
                StartCoroutine(TempStopAttackingInSeconds());
            }
        }
        IEnumerator TempStopAttackingInSeconds()
        {
            yield return new WaitForSeconds(.3f);
            currentState = EnemyState.idle;
        }
    }
}
