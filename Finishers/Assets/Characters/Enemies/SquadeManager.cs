using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Enemies
{
    public enum ManagerState { Null, Attacking }

    public class SquadeManager : MonoBehaviour
    {
        private Transform target; // target to aim for
        public ManagerState managerstate;
        public List<GameObject> enemies = new List<GameObject>();

        //[SerializeField] GameObject combatTarget = null;

        //AICharacterController character;

        void Start()
        {
            /*if (combatTarget == null)
            {
                combatTarget = GameObject.FindGameObjectWithTag("Player");
            }*/
            //character = GetComponent<AICharacterController>();
            managerstate = ManagerState.Null;
        }

        public void Startattack()
        {
            managerstate = ManagerState.Attacking;
        }

        private void OnTriggerEnter(Collider other)
        {
            
        }

    }
}
    