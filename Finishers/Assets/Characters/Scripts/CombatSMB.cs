using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    public class CombatSMB : StateMachineBehaviour
    {
        public delegate void AttackExited();
        public event AttackExited AttackExitListeners;

        void OnStateExit()
        {
            if (AttackExitListeners != null)
            {
                AttackExitListeners();
            }
        }

        //void OnStateMachineEnter()
        //{
        //    Debug.Log("Enter State");
        //}

        //// is not called when any state is used
        //void OnStateMachineExit()
        //{
        //    Debug.Log("Exit State");
        //}
    }
}
