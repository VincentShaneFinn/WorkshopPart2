using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Finisher.Characters
{
    public delegate void CharacterIsDying();

    [RequireComponent(typeof(AnimOverrideSetter))]
    public class CharacterState : MonoBehaviour
    {
        [HideInInspector] private Animator animator;

        private AnimOverrideSetter animOverrideHandler;

        void Awake()
        {
            animator = GetComponent<Animator>();
            Assert.IsNotNull(animator);

            animOverrideHandler = GetComponent<AnimOverrideSetter>();

            initialize();
        }

        private void initialize()
        {
            DyingState = new DyingState();
            Grabbing = false;
        }

        #region States that you must Get and Set from Here

        // The core idea is that something that may want to be visable from an external class should be added to the Character State From SO class overrides

        #region Dying State

        // while Dying is an aniamtion tree parameter, it should only be set through here, not via the animator
        public virtual DyingState DyingState { get; set; }
        public bool Dying { get { return DyingState.Dying; } }

        #endregion

        public virtual bool Grabbing { get; set; }

        //todo grab should be different but similar animation state from Stunned
        private bool grabbed = false;
        public bool Grabbed {
            get { return grabbed; }
            set
            {
                if (!grabbed || (grabbed == true && !Uninteruptable))
                {
                    animator.SetTrigger(AnimConstants.Parameters.RESETFORCEFULLY_TRIGGER);
                }

                animator.SetBool(AnimConstants.Parameters.STUNNED_BOOL, value);

                grabbed = value;
            }
        }

        public bool Stunned
        {
            get { return animator.GetBool(AnimConstants.Parameters.STUNNED_BOOL); }
        }

        private bool runningRecoverCR = false;
        private float recoverFromStunTime;
        public void Stun(float timeStunned, bool wasParry = false)
        {
            Systems.HealthSystem healthSystem = GetComponent<Systems.HealthSystem>();
            if (healthSystem)
            {
                healthSystem.Knockback();
            }
            else
            {
                animator.SetTrigger(AnimConstants.Parameters.RESETFORCEFULLY_TRIGGER);
            }

            Parried = wasParry;

            animator.SetBool(AnimConstants.Parameters.STUNNED_BOOL, true);
            recoverFromStunTime = Time.time + timeStunned;
            if (!runningRecoverCR) StartCoroutine(RecoverFromStun());
        }

        IEnumerator RecoverFromStun()
        {
            runningRecoverCR = true;
            yield return new WaitWhile(() => Time.time < recoverFromStunTime);
            runningRecoverCR = false;
            Parried = false;

            if (grabbed)
            {
                yield break;
            }

            animator.SetBool(AnimConstants.Parameters.STUNNED_BOOL, false);
        }

        public bool Parried { get; private set; }


    #region Invulnerable

        [HideInInspector] public bool IsDodgeFrame = false;
        [HideInInspector] public bool IsParryFrame = false;

        public bool Invulnerable
        {
            get
            {
                if (IsDodgeFrame ||
                    IsParryFrame ||
                    animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.INVULNERABLEACTION_STATE) ||
                    animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.INVULNERABLE_SEQUENCE_TAG))
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

        #endregion

        #endregion

        #region States that you can Get here, but whose animation triggers are set somewhere else

        public bool Attacking
        {
            get
            {
                return animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.LIGHTATTACK_TAG) ||
                    animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.HEAVYATTACK_TAG);
            }
        }

        public bool Dodging { get { return animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.DODGE_STATE); } }

        public bool Parrying { get { return animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.PARRY_STATE); } }

        public bool FinisherModeActive { get { return animator.GetBool(AnimConstants.Parameters.FINISHERMODE_BOOL); } }

        public bool Uninteruptable
        {
            get
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.UNINTERUPTABLE_TAG) ||
                animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.INVULNERABLE_SEQUENCE_TAG) ||
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

        #endregion

    }
}