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
        [SerializeField] private AnimatorOverrideController AnimatorOverrideControllerConfig;
        protected AnimatorOverrideController animOverrideController;

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
            Animator = gameObject.GetComponent<Animator>();

            //Setup a new AnimOverrideController

            animOverrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
            Animator.runtimeAnimatorController = animOverrideController;

            //fill new override with data
            var dataOverride = AnimatorOverrideControllerConfig;

            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(dataOverride.overridesCount);
            dataOverride.GetOverrides(overrides);

            //Apply it
            animOverrideController.ApplyOverrides(overrides);
        }

        #region Public Interface for Overriding Animation Clips

        public void SetOverride(string overrideIndex, AnimationClip animClip)
        {
            animOverrideController[overrideIndex] = animClip;
        }

        public void SetFloatOverride(string floatName, float floatValue, string overrideIndex, AnimationClip animClip)
        {
            animOverrideController[overrideIndex] = animClip;
            Animator.SetFloat(floatName, floatValue);
        }

        public void SetIntegerOverride(string intName, int intValue, string overrideIndex, AnimationClip animClip)
        {
            animOverrideController[overrideIndex] = animClip;
            Animator.SetInteger(intName, intValue);
        }

        public void SetBoolOverride(string boolName, bool boolValue, string overrideIndex, AnimationClip animClip)
        {
            animOverrideController[overrideIndex] = animClip;
            Animator.SetBool(boolName, boolValue);
        }

        public void SetTriggerOverride(string TriggerName, string OverrideIndex, AnimationClip AnimClip)
        {
            animOverrideController[OverrideIndex] = AnimClip;
            Animator.SetTrigger(TriggerName);
        }

        #endregion

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
            Animator.SetFloat(AnimConstants.Parameters.FORWARD_FLOAT, forwardAmount, 0.1f, Time.deltaTime);
            Animator.SetFloat(AnimConstants.Parameters.TURN_FLOAT, turnAmount, 0.1f, Time.deltaTime);
            Animator.SetBool(AnimConstants.Parameters.ONGROUND_BOOL, isGrounded);
            Animator.SetBool(AnimConstants.Parameters.STRAFING_BOOL, Strafing);
            if (!isGrounded)
            {
                Animator.SetFloat(AnimConstants.Parameters.JUMP_FLOAT, rigidBody.velocity.y);
            }
        }

        private void setJumpLeg()
        {
            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle =
                Mathf.Repeat(
                    Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);
            float jumpLeg = (runCycle < HALF ? 1 : -1) * forwardAmount;
            if (isGrounded)
            {
                Animator.SetFloat(AnimConstants.Parameters.FORWARDLEG_FLOAT, jumpLeg);
            }
        }

        private void setAnimatorSpeed(Vector3 move)
        {
            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (isGrounded && Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.LOCOMOTION_TAG) && move.magnitude > 0)
            {
                if (Running)
                {
                    if (Strafing)
                    {
                        if (forwardAmount >= .3f)
                        {
                            Animator.SetFloat(AnimConstants.Parameters.FORWARD_FLOAT, 2);
                        }
                        else
                        {
                            Running = false;
                        }
                        Animator.SetFloat(AnimConstants.Parameters.MOVEMENT_SPEED_MULTIPLIER, animSpeedMultiplier);
                    }
                    else
                    {
                        Animator.SetFloat(AnimConstants.Parameters.MOVEMENT_SPEED_MULTIPLIER, runAnimSpeedMultiplier);
                    }
                }
                else {
                    Animator.SetFloat(AnimConstants.Parameters.MOVEMENT_SPEED_MULTIPLIER, animSpeedMultiplier);
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
                    v = (Animator.deltaPosition * runMoveSpeedMultiplier) / Time.deltaTime;
                }
                else
                {
                    v = (Animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
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
