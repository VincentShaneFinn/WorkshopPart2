using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    // todo this class really controls all of the state variables in Character Motor, consider reverse the dependencies so this class is where you check?
    // or find a better way to handle this
    public class AnimatorStateHandler : StateMachineBehaviour
    {
        [HideInInspector] public CharacterAnimator characterAnim;

        private bool StateExited = true;
        private bool StateEntered = false;

        void OnStateEnter()
        {
            Debug.Log("Enter");
            characterAnim.CanMove = false;
            characterAnim.CanRotate = false;
        }

        void OnStateExit()
        {
            Debug.Log("Exit");
            characterAnim.CanMove = true;
            characterAnim.CanRotate = true;
        }

    }
}
