using UnityEngine;

namespace Finisher.Characters
{
    #region public Animation Constants

    public static class AnimContstants
    {
        public class States
        {
            public const string STRAFING_LOCOMOTION_STATE = "Strafing Locomotion";
            public const string BASIC_LOCOMOTION_STATE = "Basic Locomotion";
            public const string AIRBORNE_STATE = "Airborne";
            public const string STRAFING_STATE = "Strafing Locomotion";
            public const string KNOCKBACK_STATE = "Knockback";
            public const string LIGHT_ATTACK1_STATE = "Light1";
            public const string LIGHT_ATTACK2_STATE = "Light2";
            public const string LIGHT_ATTACK3_STATE = "Light3";
            public const string LIGHT_ATTACK4_STATE = "Light4";
            public const string HEAVY_ATTACK1_STATE = "Heavy1";
            public const string HEAVY_ATTACK2_STATE = "Heavy2";
            public const string DODGE_STATE = "Dodge";
            public const string DYING_STATE = "Dying";
        }

        public class Tags
        {
            public const string LOCOMOTION_TAG = "Locomotion";
            public const string ATTACKRIGHT_TAG = "AttackRight";
            public const string ATTACKLEFT_TAG = "AttackLeft";
            public const string UNINTERUPTABLE_TAG = "Uninteruptable";
        }

        public class Parameters
        {
            public const string FORWARD_FLOAT = "Forward";
            public const string TURN_FLOAT = "Turn";
            public const string JUMP_FLOAT = "Jump";
            public const string FORWARDLEG_FLOAT = "ForwardLeg";

            public const string ONGROUND_BOOL = "OnGround";
            public const string STRAFING_BOOL = "Strafing";
            public const string ISHEAVY_BOOL = "IsHeavy";
            public const string DYING_BOOL = "Dying";

            public const string ATTACK_TRIGGER = "Attack";
            public const string DODGE_TRIGGER = "Dodge";
            public const string KNOCKBACK_TRIGGER = "Knockback";

            public const string ATTACK_SPEED_MULTIPLIER = "AttackSpeedMultiplier";
            public const string MOVEMENT_SPEED_MULTIPLIER = "MovementSpeedMultiplier";
        }

        public static class OverrideIndexes
        {
            public const string KNOCKBACK_INDEX = "DEFAULT_KNOCKBACK";
            public const string DODGE_INDEX = "DEFAULT_DODGE";
        }
    }

    #endregion

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
            animator.SetFloat(AnimContstants.Parameters.FORWARD_FLOAT, forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat(AnimContstants.Parameters.TURN_FLOAT, turnAmount, 0.1f, Time.deltaTime);
            animator.SetBool(AnimContstants.Parameters.ONGROUND_BOOL, isGrounded);
            animator.SetBool(AnimContstants.Parameters.STRAFING_BOOL, Strafing);
            if (!isGrounded)
            {
                animator.SetFloat(AnimContstants.Parameters.JUMP_FLOAT, rigidBody.velocity.y);
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
                animator.SetFloat(AnimContstants.Parameters.FORWARDLEG_FLOAT, jumpLeg);
            }
        }

        // todo can we have a way to override this
        // todo this isnt great
        private void setAnimatorSpeed(Vector3 move)
        {
            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.LOCOMOTION_TAG) && move.magnitude > 0)
            {
                if (Running)
                {
                    animator.SetFloat(AnimContstants.Parameters.MOVEMENT_SPEED_MULTIPLIER, runAnimSpeedMultiplier);
                }
                else {
                    animator.SetFloat(AnimContstants.Parameters.MOVEMENT_SPEED_MULTIPLIER, animSpeedMultiplier);
                }
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

        #region Movement Animation Events

        public void FootL()
        {
            //play left foot sound
        }

        public void FootR()
        {
            //play right foot sound
        }

        #endregion

    }
}
