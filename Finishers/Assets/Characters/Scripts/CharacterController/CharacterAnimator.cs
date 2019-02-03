using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{

    public class CharacterAnimator : CharacterMotor
	{

        #region Character Animator Variables

        [Header("Animation Settings")]
        [Tooltip("Used to move faster, using animation speed")]
        [SerializeField] protected float animSpeedMultiplier = 1f;
        [Tooltip("Used to move faster, using animation speed")]
        [SerializeField] protected float runAnimSpeedMultiplier = 1.6f;

        public CharAnimStateHandler stateHandler { get; private set; }

        #endregion

        protected override void Awake()
        {
            base.Awake();

            //Add Components
            stateHandler = gameObject.AddComponent<CharAnimStateHandler>();

            componentGetter();
        }

        private void componentGetter()
        {
            //Get Components
            animator = gameObject.GetComponent<Animator>();
        }

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
            animator.SetFloat(AnimConstants.Parameters.FORWARD_FLOAT, forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat(AnimConstants.Parameters.TURN_FLOAT, turnAmount, 0.1f, Time.deltaTime);
            if (characterState.Grabbing)
            {
                isGrounded = true;
            }
            animator.SetBool(AnimConstants.Parameters.ONGROUND_BOOL, isGrounded);
            animator.SetBool(AnimConstants.Parameters.STRAFING_BOOL, Strafing);
            if (!isGrounded)
            {
                animator.SetFloat(AnimConstants.Parameters.JUMP_FLOAT, rigidBody.velocity.y);
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
                animator.SetFloat(AnimConstants.Parameters.FORWARDLEG_FLOAT, jumpLeg);
            }
        }

        private void setAnimatorSpeed(Vector3 move)
        {
            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.LOCOMOTION_TAG) && move.magnitude > 0)
            {
                if (Running)
                {
                    if (Strafing)
                    {
                        if (forwardAmount >= .3f)
                        {
                            animator.SetFloat(AnimConstants.Parameters.FORWARD_FLOAT, 2);
                        }
                        else
                        {
                            Running = false;
                        }
                        animator.SetFloat(AnimConstants.Parameters.MOVEMENT_SPEED_MULTIPLIER, animSpeedMultiplier);
                    }
                    else
                    {
                        animator.SetFloat(AnimConstants.Parameters.MOVEMENT_SPEED_MULTIPLIER, runAnimSpeedMultiplier);
                    }
                }
                else {
                    animator.SetFloat(AnimConstants.Parameters.MOVEMENT_SPEED_MULTIPLIER, animSpeedMultiplier);
                }
            }
            else
            {
                Running = false;
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
                if (Running && !Strafing)
                {
                    v = (animator.deltaPosition * runMoveSpeedMultiplier) / Time.deltaTime;
                }
                else
                {
                    v = (animator.deltaPosition * MovementSpeedMultiplier) / Time.deltaTime;
                }
                // we preserve the existing y part of the current velocity.
                v.y = rigidBody.velocity.y;
                rigidBody.velocity = v;
            }
        }

        #endregion

        #region Movement Animation Events

        void FootL()
        {
            //play left foot sound
        }

        void FootR()
        {
            //play right foot sound
        }

        #endregion

    }
}
