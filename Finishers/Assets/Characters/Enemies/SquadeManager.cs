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
        [HideInInspector] public ManagerState CurrentManagerState;
        private EnemyAI[] enemies;

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
            setEnemies();
        }

        private void setEnemies()
        {
            enemies = GetComponentsInChildren<EnemyAI>();
        }

        public void SendWakeUpCallToEnemies()
        {
            CurrentManagerState = ManagerState.Attacking;
            CallWakeUpListeners();
        }

        private void OnTriggerEnter(Collider other)
        {
            
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
    