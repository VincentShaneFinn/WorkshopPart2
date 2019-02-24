using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Finisher.Characters.Enemies.Systems;
using Finisher.Core;

namespace Finisher.Characters.Enemies
{
    public enum ManagerState { Waiting, Attacking, ReturnHome }

    public class SquadManager : MonoBehaviour
    {
        [SerializeField] private int directAttackers = 1;
        [HideInInspector] public int DirectAttackers { get { return directAttackers; } }
        [SerializeField] private int indirectAttackers = 2;
        [HideInInspector] public int IndirectAttackers { get { return indirectAttackers; } }

        [HideInInspector]public ManagerState CurrentManagerState;
        private List<GameObject> enemies = new List<GameObject>();
        private GameObject player;
        private EnemyManager enemyManager;
        private CharacterStateSO playerState;
        private KnightLeaderAI leader;

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
            enemyManager = FindObjectOfType<EnemyManager>();

            setEnemies();
        }

        private void setEnemies()
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Enemy")
                {
                    enemies.Add(child.gameObject);
                    var knightLeader = child.gameObject.GetComponent<KnightLeaderAI>();
                    if (knightLeader)
                    {
                        leader = knightLeader;
                    }
                }
            }
            SortEnemiesByDistance();
        }

        IEnumerator assignEnemyRoles()
        {
            while (player)
            {
                setEnemiesSubChase();
                yield return new WaitForSeconds(0.5f);
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
                enemyManager.AddCombatSquad(this);
                StartCoroutine(assignEnemyRoles()); //Move to play 1 second after first enemy starts chasing
            }
        }
        
        public void RemoveEnemy(GameObject enemy)
        {
            enemies.Remove(enemy);
            if(enemies.Count <= 0)
            {
                enemyManager.RemoveCombatSquad(this);
            }
        }

        private void setEnemiesSubChase()
        {
            SortEnemiesByDistance();
            var directAttackersCount = directAttackers;
            if (leader)
            {
                var healthSystem = leader.GetComponent<EnemyLeaderHealthSystem>();
                if (healthSystem && healthSystem.GetHealthAsPercent() <= .3f)
                {
                    directAttackersCount++;
                }
            }
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
                    if (Ai.currentChaseSubstate != ChaseSubState.Arced)
                    {
                        var positive = UnityEngine.Random.value > 0.5f;
                        if (positive)
                        {
                            Ai.ArcAngle = UnityEngine.Random.Range(40, 50);
                        }
                        else
                        {
                            Ai.ArcAngle = UnityEngine.Random.Range(-50, -40);
                        }
                    }
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
                resetEnemiesChaseSubstates();
                enemyManager.RemoveCombatSquad(this);
            }
        }

        private void resetEnemiesChaseSubstates()
        {
            foreach (GameObject enemy in enemies)
            {
                EnemyAI Ai = enemy.GetComponent<EnemyAI>();
                Ai.currentChaseSubstate = ChaseSubState.Direct;
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
    