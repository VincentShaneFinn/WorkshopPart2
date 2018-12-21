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

        //void Update()
        //{
        //    // myAnimator.GetAnimatorTransitionInfo(0).IsName("flight -> shot") // get current transition
        //    var animState = animator.GetCurrentAnimatorStateInfo(0);

        //    if (animState.IsName(CharAnimStates.DYING_STATE))
        //    {
        //        //PreventMovement();
        //        return;
        //    }

        //    if (animState.IsName(CharAnimStates.KNOCKBACK_STATE))
        //    {
        //        //ClearActionTriggers();
        //        //PreventMovement();
        //        return;
        //    }
        //    else if (animState.IsName(CharAnimStates.ATTACK_STATE)){
        //        //ClearActionTriggers();
        //        //PreventMovement(overrideCanAct: true, canDodge: true);
        //    }
        //    else if (animState.IsName(CharAnimStates.DODGE_STATE)){
        //        //ClearActionTriggers();
        //        //PreventMovement();
        //    }
        //    else if (animState.IsName(CharAnimStates.LOCOMOTION_STATE) || 
        //             animState.IsName(CharAnimStates.STRAFING_STATE))
        //    {
        //        //FreeAllMovement();
        //    }
        //    if (animator.IsInTransition(0) && animator.GetAnimatorTransitionInfo(0).anyState)
        //    {
        //        //PreventMovement();
        //    }
        //}

        private void FreeAllMovement()
        {
            characterAnim.CanMove = true;
            characterAnim.CanRotate = true;
            characterAnim.CanJump = true;
            characterAnim.CanAct = true;
            characterAnim.CanDodge = true;
        }

        private void PreventMovement(bool canMove = false, bool canRotate = false, bool canJump = false, bool canAct = false, bool canDodge = false, bool overrideCanAct = false)
        {
            characterAnim.CanMove = canMove;
            characterAnim.CanRotate = canRotate;
            characterAnim.CanJump = canJump;
            if (!overrideCanAct) // the combat system controlls the can Act during this time
            {
                characterAnim.CanAct = canAct;
            }
            characterAnim.CanDodge = canDodge;
        }

        private void ClearActionTriggers()
        {
            //animator.ResetTrigger("Attack");
        }

    }
}
