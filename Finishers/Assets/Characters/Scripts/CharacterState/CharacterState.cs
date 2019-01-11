using UnityEngine;
using UnityEngine.Assertions;

namespace Finisher.Characters
{
    public delegate void CharacterIsDying();

    [RequireComponent(typeof(AnimOverrideHandler))]
    public class CharacterState : MonoBehaviour
    {
        [HideInInspector] private Animator animator;

        private AnimOverrideHandler animOverrideHandler;

        void Awake()
        {
            animator = GetComponent<Animator>();
            Assert.IsNotNull(animator);

            animOverrideHandler = GetComponent<AnimOverrideHandler>();

            initialize();
        }

        private void initialize()
        {
            DyingState = new DyingState();
            Grabbing = false;
        }

        // The core idea is that something that may want to be visable from an external class should be added to the Character State From SO class overrides

        #region Dying State

        // while Dying is an aniamtion tree parameter, it should only be set through here, not via the animator
        public virtual DyingState DyingState { get; set; }
        public bool Dying { get { return DyingState.Dying; } }

        #endregion

        public virtual bool Grabbing { get; set; }

        public bool Stunned
        {
            get { return animator.GetBool(AnimConstants.Parameters.STUNNED_BOOL); }
            set
            {
                if (!Uninteruptable)
                {
                    animator.SetTrigger(AnimConstants.Parameters.RESETFORCEFULLY_TRIGGER);
                }
                animator.SetBool(AnimConstants.Parameters.STUNNED_BOOL, value);
            }
        }

        public bool Attacking
        {
            get
            {
                return animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.LIGHTATTACK_TAG) ||
                    animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.HEAVYATTACK_TAG);
            }
        }

        public bool Dodging { get { return animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.DODGE_STATE); } }

        public bool FinisherModeActive { get { return animator.GetBool(AnimConstants.Parameters.FINISHERMODE_BOOL); } }

        public bool Uninteruptable
        {
            get
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.UNINTERUPTABLE_TAG) ||
                animator.GetAnimatorTransitionInfo(0).anyState ||
                Stunned ||
                Grabbing)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Invulnerable
        {
            get
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.DODGE_STATE) ||
                    animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.INVULNERABLEACTION_STATE))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public void EnterInvulnerableActionState(AnimationClip animClip)
        {
            animOverrideHandler.SetTriggerOverride(AnimConstants.Parameters.INVULNERABLEACTION_TRIGGER,
            AnimConstants.OverrideIndexes.INVULNERABLEACTION_INDEX,
            animClip);
        }
    }
}