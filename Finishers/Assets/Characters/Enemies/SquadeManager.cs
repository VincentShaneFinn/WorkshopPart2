using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Finisher.Characters.Enemies
{
    public enum ManagerState { Waiting, Attacking }

    public class SquadeManager : MonoBehaviour
    {
        //private Transform target; // target to aim for
        private ManagerState managerstate;
        private List<EnemyAI> enemies = new List<EnemyAI>();

        //[SerializeField] GameObject combatTarget = null;

        //AICharacterController character;

        void Start()
        {
            managerstate = ManagerState.Waiting;
            setEnemies();
        }

        private void setEnemies()
        {
            enemies = GetComponentsInChildren<EnemyAI>().ToList();
        }   

        public void SendWakeUpCallToEnemies()
        {
            managerstate = ManagerState.Attacking;
            foreach (EnemyAI enemy in enemies)
            {
                enemy.AttackByManager();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                foreach (EnemyAI enemy in enemies)
                {
                    enemy.StopByManager();    
                }
            }
        }

    }
}
    