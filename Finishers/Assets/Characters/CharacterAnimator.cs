using System;
using System.Collections;
using UnityEngine;

namespace Finisher.Characters
{
	public class CharacterAnimator : CharacterMotor
	{

        #region Movement Animation Control
        protected override void updateAnimator(Vector3 move)
        {
            updateAnimatorParams();
            setJumpLeg();
            setAnimatorSpeed(move);
        }

        private void updateAnimatorParams()
        {
            // update the animator parameters
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.SetBool("OnGround", isGrounded);
            animator.SetBool("Strafing", Strafing);
            if (!isGrounded)
            {
                animator.SetFloat("Jump", rigidBody.velocity.y);
            }
        }

        private void setJumpLeg()
        {
            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle =
                Mathf.Repeat(
                    animator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);
            float jumpLeg = (runCycle < HALF ? 1 : -1) * forwardAmount;
            if (isGrounded)
            {
                animator.SetFloat("JumpLeg", jumpLeg);
            }
        }

        private void setAnimatorSpeed(Vector3 move)
        {
            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName(LOCOMOTION_STATE) && move.magnitude > 0)
            {
                if (!Running)
                {
                    animator.speed = animSpeedMultiplier;
                }
                else
                    animator.speed = runAnimSpeedMultiplier;
            }
            else
            {
                // don't use that while airborne
                animator.speed = 1;
            }
        }

        void OnAnimatorMove()
        {
            modifyPositionalVelocity();
        }

        private void modifyPositionalVelocity()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (isGrounded && Time.deltaTime > 0)
            {
                Vector3 v;
                if (Running)
                {
                    v = (animator.deltaPosition * runMoveSpeedMultiplier) / Time.deltaTime;
                }
                else
                {
                    v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
                }

                // we preserve the existing y part of the current velocity.
                if (rigidBody.velocity.y > 0) // protect from going to fast up hill
                {
                    v.y = 0;
                }
                else
                {
                    v.y = rigidBody.velocity.y;
                }
                rigidBody.velocity = v;
                if (isGrounded)
                {
                    restrictYVelocity();
                }

            }
        }

        void restrictYVelocity()
        {
            float newYVelocity = rigidBody.velocity.y;
            if (rigidBody.velocity.y > 0)
                newYVelocity = 0;

            rigidBody.velocity = new Vector3(rigidBody.velocity.x, newYVelocity, rigidBody.velocity.z);
        }

        #endregion

        // todo move all of this stuff somewhere else since this really is used to animate the character during movement

        public void Hit()
        {
            print("hit something now");
        }

        public void FootL()
        {

        }

        public void FootR()
        {

        }

        // todo protect triggers from being set twice

        public void Kill()
        {
            if (Dying) { return; }
            Dying = true;
            animator.SetBool("Dying",true);
        }


        //todo, seperate into player and enemy combat systems
        public void Attack(bool canMove = false, bool canRotate = false)
        {
            if (CanAct)
            {
                animator.SetTrigger("Attack");
                RestrictMovementDuringAnimation(canMove, canRotate);
            }
        }

        public void Dodge(bool canMove = true, bool canRotate = false)
        {
            if (CanAct)
            {
                animator.SetTrigger("Dodge");
                RestrictMovementDuringAnimation(canMove, canRotate);
            }
        }

        public void Knockback(bool canMove = false, bool canRotate = false)
        {
            animator.SetTrigger("Knockback");
            RestrictMovementDuringAnimation(canMove, canRotate);
            // todo add a knocback factor
            // todo add a knockback move direction
        }

        private void RestrictMovementDuringAnimation(bool canMove, bool canRotate)
        {
            CanAct = false;
            CanMove = canMove;
            CanRotate = canRotate;
            //free it after the animation is complete
            StartCoroutine(RestoreAllMovement());
        }

        private IEnumerator RestoreAllMovement()
        {
            float currentAnimationTime = .5f;
            yield return new WaitForSeconds(currentAnimationTime);
            CanMove = true;
            CanRotate = true;
            CanAct = true;
        }
    }
}
