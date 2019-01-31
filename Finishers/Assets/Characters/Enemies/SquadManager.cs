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

        public delegate void EnemiesEngage();
        public event EnemiesEngage OnEnemiesEngage;
        public void CallWakeUpListeners()
        {
            if (OnEnemiesEngage != null)
            {
                OnEnemiesEngage();
                OnEnemiesSubChase();
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
            enemies = enemies.OrderBy( x => Vector2.Distance(this.transform.position, x.transform.position) ).ToList();
            
        }

        public void SendWakeUpCallToEnemies()
        {
            CurrentManagerState = ManagerState.Attacking;
            CallWakeUpListeners();
        }
        
        public void RemoveEnemy(GameObject enemy)
        {
            enemies.Remove(enemy);
            OnEnemiesSubChase();
        }

        public void OnEnemiesSubChase()
        {
            int X = enemies.Count();
            int x = 0;
            foreach (GameObject enemy in enemies)
            {
                EnemyAI Ai = enemy.GetComponent<EnemyAI>();
                if (x < 2) { Ai.currentChaseSubstate = ChaseSubState.Direct; }
                else if (x < 4) { Ai.currentChaseSubstate = ChaseSubState.Arced; }
                else { Ai.currentChaseSubstate = ChaseSubState.Surround; }
                x += 1;
               
            }
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
    