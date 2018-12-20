using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    public class AnimatorStateHandler : MonoBehaviour
    {
        CharacterAnimator characterAnim;
        Animator animator;

        private bool canJumpInitial;

        void Start()
        {
            characterAnim = GetComponent<CharacterAnimator>();
            canJumpInitial = characterAnim.CanJump;

            animator = GetComponent<Animator>();
        }

        void Update()
        {
            // myAnimator.GetAnimatorTransitionInfo(0).IsName("flight -> shot") // get current transition
            var animState = animator.GetCurrentAnimatorStateInfo(0);

            if (animState.IsName(CharAnimStates.DYING_STATE))
            {
                characterAnim.CanMove = false;
                characterAnim.CanRotate = false;
                characterAnim.CanAct = false;
                characterAnim.CanJump = false;
                return;
            }
            if (animState.IsName(CharAnimStates.KNOCKBACK_STATE))
            {
                ClearAllTriggers();
                PreventMovement();
            }
            else if (animState.IsName(CharAnimStates.ATTACK1_STATE)){
                ClearAllTriggers();
                PreventMovement();
            }
            else if (animState.IsName(CharAnimStates.DODGE_STATE)){
                ClearAllTriggers();
                PreventMovement();
            }
            else if (animState.IsName(CharAnimStates.LOCOMOTION_STATE) || 
                animState.IsName(CharAnimStates.STRAFING_STATE))
            {
                characterAnim.CanMove = true;
                characterAnim.CanRotate = true;
                characterAnim.CanJump = true;
                characterAnim.CanAct = true;
            }
            //if (animator.IsInTransition(0))
            //{

            //}
        }

        private void PreventMovement(bool canMove = false, bool canRotate = false, bool canJump = false, bool canAct = false)
        {
            characterAnim.CanMove = canMove;
            characterAnim.CanRotate = canRotate;
            characterAnim.CanJump = canJump;
            characterAnim.CanAct = canAct;
        }

        private void ClearAllTriggers()
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Dodge");
        }

    }
}
