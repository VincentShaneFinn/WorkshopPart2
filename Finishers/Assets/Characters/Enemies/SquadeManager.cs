using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Finisher.Characters.Enemies
{
    public enum ManagerState { Waiting, Attacking }

    public class SquadeManager : MonoBehaviour
    {
        //private Transform target; // target to aim for
        [HideInInspector] public ManagerState ManagerState;
        private EnemyAI[] enemies;

        //[SerializeField] GameObject combatTarget = null;

        //AICharacterController character;

        void Start()
        {
            ManagerState = ManagerState.Waiting;
            setEnemies();
        }

        private void setEnemies()
        {
            enemies = GetComponentsInChildren<EnemyAI>();
        }

        public void SendWakeUpCallToEnemies()
        {
            ManagerState = ManagerState.Attacking;
            StartCoroutine(AlertEnemies());
        }

        IEnumerator AlertEnemies()
        {
            foreach (EnemyAI enemy in enemies)
            {
                yield return null;
                enemy.AttackByManager();
            }
        }

        IEnumerator FreeEnemies()
        {
            foreach (EnemyAI enemy in enemies)
            {
                yield return null;
                enemy.StopByManager();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                StartCoroutine(FreeEnemies());
            }
        }

    }
}
    