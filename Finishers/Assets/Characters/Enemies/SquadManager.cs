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
        [SerializeField] private int indirectAttackers = 1;

        [HideInInspector]public ManagerState CurrentManagerState;
        private List<GameObject> enemies = new List<GameObject>();
        private GameObject player;
        private CharacterStateSO playerState;

        private List<KnightAI> enemiesThatRushed = new List<KnightAI>();

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
            sortEnemiesByDistance();
        }

        IEnumerator assignEnemyRoles()
        {
            while (player)
            {
                setEnemiesSubChase();
                yield return new WaitForSeconds(1f);
            }
        }

        IEnumerator issueTeamRushAttack()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 5));
            yield return StartCoroutine(teamRushAttackSequence());
            StartCoroutine(issueTeamRushAttack());
        }

        IEnumerator teamRushAttackSequence()
        {
            enemiesThatRushed = new List<KnightAI>();

            int numberOfRushers = 3;

            while(numberOfRushers > 0)
            {
                KnightAI knight = getAIForRushAttack();
                if (!knight) { yield break; }
                enemiesThatRushed.Add(knight);
                knight.PerformRushAttack();
                numberOfRushers--;
                yield return new WaitForSeconds(1.5f);
            }
        }

        private KnightAI getAIForRushAttack()
        {
            if (enemies.Count <= directAttackers + indirectAttackers) { return null; }

            sortEnemiesByDistance();

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var knight = enemies[enemies.Count - 1].GetComponent<KnightAI>();
                if (knight)
                {
                    return knight;
                }
            }
            return null;
        }

        public void SendWakeUpCallToEnemies()
        {
            if (CurrentManagerState != ManagerState.Attacking)
            {
                CurrentManagerState = ManagerState.Attacking;
                StartCoroutine(issueTeamRushAttack());
                StartCoroutine(assignEnemyRoles()); //Move to play 1 second after first enemy starts chasing
            }
        }
        
        public void RemoveEnemy(GameObject enemy)
        {
            enemies.Remove(enemy);
        }

        private void setEnemiesSubChase()
        {
            sortEnemiesByDistance();
            int index = 0;
            foreach (GameObject enemy in enemies)
            {
                EnemyAI Ai = enemy.GetComponent<EnemyAI>();
                if (index < directAttackers) { Ai.currentChaseSubstate = ChaseSubState.Direct; }
                else if (index < indirectAttackers + directAttackers) { Ai.currentChaseSubstate = ChaseSubState.Arced; }
                else { Ai.currentChaseSubstate = ChaseSubState.Surround; }
                index++;
            }
        }

        private void sortEnemiesByDistance()
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
    