using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Finisher.Characters.Enemies
{
    public enum ManagerState { Waiting, Attacking, ReturnHome }

    public class SquadManager : MonoBehaviour
    {
        [SerializeField] private int directAttackers = 1;
        [HideInInspector] public int DirectAttackers { get { return directAttackers; } }
        [SerializeField] private int indirectAttackers = 1;
        [HideInInspector] public int IndirectAttackers { get { return indirectAttackers; } }

        [HideInInspector]public ManagerState CurrentManagerState;
        private List<GameObject> enemies = new List<GameObject>();
        private GameObject player;
        private CharacterStateSO playerState;

        //public delegate void EnemiesEngage();
        //public event EnemiesEngage OnEnemiesEngage;
        //public void CallWakeUpListeners()
        //{
        //    if (OnEnemiesEngage != null)
        //    {
        //        OnEnemiesEngage();
        //    }
        //}

        void Start()
        {
            CurrentManagerState = ManagerState.ReturnHome;
            player = GameObject.FindGameObjectWithTag(TagNames.PlayerTag);

            if (player) {
                playerState = player.GetComponent<CharacterStateFromSO>().stateSO;
            }

            setEnemies();
        }

        private void setEnemies()
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Enemy")
                {
                    enemies.Add(child.gameObject);
                }
            }
            SortEnemiesByDistance();
        }

        IEnumerator assignEnemyRoles()
        {
            while (player)
            {
                setEnemiesSubChase();
                yield return new WaitForSeconds(1f);
            }
        }

        public List<GameObject> GetEnemies()
        {
            return enemies;
        }

        public void SendWakeUpCallToEnemies()
        {
            if (CurrentManagerState != ManagerState.Attacking)
            {
                CurrentManagerState = ManagerState.Attacking;
                StartCoroutine(assignEnemyRoles()); //Move to play 1 second after first enemy starts chasing
            }
        }
        
        public void RemoveEnemy(GameObject enemy)
        {
            enemies.Remove(enemy);
        }

        private void setEnemiesSubChase()
        {
            SortEnemiesByDistance();
            var directAttackersCount = directAttackers;
            var indirectAttackersCount = indirectAttackers;
            foreach (GameObject enemy in enemies)
            {
                EnemyAI Ai = enemy.GetComponent<EnemyAI>();
                if(Ai is KnightLeaderAI)
                {
                    Ai.currentChaseSubstate = ChaseSubState.Surround;
                    continue;
                }

                if (directAttackersCount > 0) {
                    Ai.currentChaseSubstate = ChaseSubState.Direct;
                    directAttackersCount--;
                }
                else if (indirectAttackersCount > 0) {
                    Ai.currentChaseSubstate = ChaseSubState.Arced;
                    indirectAttackersCount--;
                }
                else { Ai.currentChaseSubstate = ChaseSubState.Surround; }
            }
        }

        public void SortEnemiesByDistance()
        {
            enemies = enemies.OrderBy(x => Vector2.Distance(player.transform.position, x.transform.position)).ToList();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                CurrentManagerState = ManagerState.ReturnHome;
                StopAllCoroutines();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                CurrentManagerState = ManagerState.Waiting;
            }
        }

    }
}
    