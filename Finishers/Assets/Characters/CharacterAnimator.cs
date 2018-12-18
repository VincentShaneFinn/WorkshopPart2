using System;
using System.Collections;
using UnityEngine;

namespace Finisher.Characters
{
	public class CharacterAnimator : CharacterMotor
	{

        protected override void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
			animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
			animator.SetBool("OnGround", isGrounded);
			if (!isGrounded)
			{
				animator.SetFloat("Jump", rigidBody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat(
					animator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);
			float jumpLeg = (runCycle < half ? 1 : -1) * forwardAmount;
			if (isGrounded)
			{
				animator.SetFloat("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName(LOCOMOTION_STATE) && move.magnitude > 0)
			{
                if (!isRunning)
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

        protected override void AttemptToJump(bool jump)
        {
            // check whether conditions are right to allow a jump:
            if (jump && animator.GetCurrentAnimatorStateInfo(0).IsName(LOCOMOTION_STATE) && !animator.GetAnimatorTransitionInfo(0).anyState)
            {
                // jump!
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpPower, rigidBody.velocity.z);
                isGrounded = false;
                animator.applyRootMotion = false;
                groundCheckDistance = 0.1f;

                if (!RecentlyJumped)
                {
                    RecentlyJumped = true;
                    float timeToFreeJump = .3f;
                    StartCoroutine(FreeJumpOverTime(timeToFreeJump));
                }
            }
        }

        IEnumerator FreeJumpOverTime(float time)
        {
            yield return new WaitForSeconds(time);
            RecentlyJumped = false;
        }

        void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (isGrounded && Time.deltaTime > 0)
			{
                Vector3 v;
                if (isRunning)
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
                    RestrictYVelocity();
                }
			}
		}
    }
}
