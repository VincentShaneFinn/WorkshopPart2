using UnityEngine;
using UnityEngine.Assertions;

namespace Finisher.Characters
{
    public delegate void CharacterIsDying();

    public class CharacterState : MonoBehaviour
    {
        [HideInInspector] public Animator Animator;

        protected void Awake()
        {
            Animator = GetComponent<Animator>();
            Assert.IsNotNull(Animator);

            initialize();
        }

        protected void initialize()
        {
            DyingBool = new DyingState(Animator);
            Grabbing = false;
        }

        // The core idea is that something that may want to be visable from an external class should be added to the Character State From SO class overrides

        #region States Stored in Scriptable Object

        // while Dying is an aniamtion tree parameter, it should only be set through here, not via the animator
        public virtual DyingState DyingBool { get; set; }

        public virtual bool Grabbing { get; set; }

        #endregion

        #region Public Interface

        public bool Dying { get { return DyingBool.Dying; } set { DyingBool.Dying = value; } }

        public bool Stunned
        {
            get { return Animator.GetBool(AnimConstants.Parameters.STUNNED_BOOL); }
            set
            {
                if (!Uninteruptable)
                {
                    Animator.SetTrigger(AnimConstants.Parameters.RESETFORCEFULLY_TRIGGER);
                }
                Animator.SetBool(AnimConstants.Parameters.STUNNED_BOOL, value);
            }
        }

        public bool Attacking
        {
            get
            {
                return Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.LIGHTATTACK_TAG) ||
                    Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.HEAVYATTACK_TAG);
            }
        }
        public bool Dodging { get { return Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.DODGE_STATE); } }
        public bool FinisherModeActive { get { return Animator.GetBool(AnimConstants.Parameters.FINISHERMODE_BOOL); } }

        public bool Uninteruptable
        {
            get
            {
                if (Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.UNINTERUPTABLE_TAG) ||
                Animator.GetAnimatorTransitionInfo(0).anyState ||
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
                if (Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.DODGE_STATE) ||
                    Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.INVULNERABLEACTION_STATE))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion


    }
}