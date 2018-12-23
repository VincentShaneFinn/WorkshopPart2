using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    // todo this class really controls all of the state variables in Character Motor, consider reverse the dependencies so this class is where you check?
    // or find a better way to handle this
    public class CombatSMB : StateMachineBehaviour
    {
        public delegate void DamageFrameChanged(bool isDamageFrame);
        public event DamageFrameChanged OnDamageFrameChanged;


        void OnStateExit()
        {
            Debug.Log("State left");
        }

        void OnStateMachineEnter()
        {
            Debug.Log("Enter State");
        }

        void OnStateMachineExit()
        {
            Debug.Log("Exit State");
        }
    }
}
