using UnityEngine;

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
        SquadeManager manager;
        private CombatSystem combatSystem;
        public EnemyState state;
        private Transform Hometargrt;


        // Use this for initialization
        void Start()
        {
            Hometargrt = this.transform;
            home.transform.position = Hometargrt.position;
            if (combatTarget == null)
            {
                combatTarget = GameObject.FindGameObjectWithTag("Player");
            }
            character = GetComponent<AICharacterController>();
            combatSystem = GetComponent<CombatSystem>();
            state = EnemyState.idle;
            
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(home.transform.position);
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
            if (distanceToPlayer <= chaseRadius)
            {
                character.SetTarget(combatTarget.transform);
                state = EnemyState.Chasing;
                //manager.Startattack();
            }
            else
            {
                character.SetTarget(home.transform);
                //Debug.Log(Hometargrt.transform.position);
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
    }
}
