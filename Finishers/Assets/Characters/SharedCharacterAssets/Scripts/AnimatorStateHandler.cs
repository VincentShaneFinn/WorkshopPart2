using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    // todo this class really controls all of the state variables in Character Motor, consider reverse the dependencies so this class is where you check?
    // or find a better way to handle this
    public class AnimatorStateHandler : StateMachineBehaviour
    {

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
