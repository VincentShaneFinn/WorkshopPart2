using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Enemies
{
    public enum ManagerState { Waiting, Attacking }

    public class SquadeManager : MonoBehaviour
    {
        private Transform target; // target to aim for
        public ManagerState managerstate;
        public List<EnemyAI> enemies = new List<EnemyAI>();

        //[SerializeField] GameObject combatTarget = null;

        //AICharacterController character;

        void Start()
        {
            managerstate = ManagerState.Waiting;
        }

        

        public void Startattack()
        {
            managerstate = ManagerState.Attacking;
            //Debug.Log("Attacking!");
            foreach (EnemyAI item in enemies)
            {
                item.AttackByManager();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                foreach (EnemyAI item in enemies)
                {
                    item.StopByManager();    
                }
            }
        }

    }
}
    