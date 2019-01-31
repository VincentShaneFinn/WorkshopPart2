using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Finisher.Characters.Enemies
{
    public enum ManagerState { Waiting, Attacking }

    public class SquadManager : MonoBehaviour
    {
        //private Transform target; // target to aim for
        [HideInInspector]public ManagerState CurrentManagerState;
        private List<GameObject> enemies = new List<GameObject>();
        private GameObject player;

        public delegate void EnemiesEngage();
        public event EnemiesEngage OnEnemiesEngage;
        public void CallWakeUpListeners()
        {
            if (OnEnemiesEngage != null)
            {
                OnEnemiesEngage();
            }
        }

        public delegate void EnemiesDisengage();
        public event EnemiesDisengage OnEnemiesDisengage;
        public void CallReturnHomeListeners()
        {
            if (OnEnemiesDisengage != null)
            {
                OnEnemiesDisengage();
            }
        }

        //[SerializeField] GameObject combatTarget = null;

        //AICharacterController character;

        void Start()
        {
            CurrentManagerState = ManagerState.Waiting;
            player = GameObject.FindGameObjectWithTag(TagNames.PlayerTag);
            setEnemies();
        }

        private void setEnemies()
        {
            //enemies = GetComponentsInChildren<EnemyAI>();
            foreach (Transform child in transform)
            {
                if (child.tag == "Enemy")
                {
                    enemies.Add(child.gameObject);
                }
            }
            sortEnemyByDistance();
        }

        void Update()
        {
            setEnemiesSubChase();
        }

        public void SendWakeUpCallToEnemies()
        {
            CurrentManagerState = ManagerState.Attacking;
            CallWakeUpListeners();
        }
        
        public void RemoveEnemy(GameObject enemy)
        {
            enemies.Remove(enemy);
        }

        private void setEnemiesSubChase()
        {
            sortEnemyByDistance();
            int x = 0;
            foreach (GameObject enemy in enemies)
            {
                EnemyAI Ai = enemy.GetComponent<EnemyAI>();
                if (x < 2) { Ai.currentChaseSubstate = ChaseSubState.Direct; }
                else if (x < 4) { Ai.currentChaseSubstate = ChaseSubState.Arced; }
                else { Ai.currentChaseSubstate = ChaseSubState.Surround; }
                x++;
            }
        }

        private void sortEnemyByDistance()
        {
            enemies = enemies.OrderBy(x => Vector2.Distance(player.transform.position, x.transform.position)).ToList();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                CallReturnHomeListeners();
                CurrentManagerState = ManagerState.Waiting;
            }
        }

    }
}
    