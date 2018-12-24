using System.Collections;
using UnityEngine;

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

        AICharacterController aiCharacter;
        private CombatSystem combatSystem;

        private EnemyState currentState;


        // Use this for initialization
        void Start()
        {
            if (combatTarget == null)
            {
                combatTarget = GameObject.FindGameObjectWithTag("Player");
            }
            aiCharacter = GetComponent<AICharacterController>();
            combatSystem = GetComponent<CombatSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            testInput();
            // todo make a state machine
            pursueNearbyPlayer();
            if (currentState != EnemyState.Attacking && combatSystem.isActiveAndEnabled) // should be in range, then start attacking if we arent already
            {
                attackPlayerIfNear();
            }
        }

        private void testInput()
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
        }

        private void pursueNearbyPlayer()
        {
            float distanceToPlayer = Vector3.Distance(combatTarget.transform.position, transform.position);
            if (distanceToPlayer <= chaseRadius)
            {
                aiCharacter.SetTarget(combatTarget.transform);
            }
            else
            {
                aiCharacter.SetTarget(transform);
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
