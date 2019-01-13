using System.Collections;
using UnityEngine;

namespace Finisher.Characters.Systems
{

    public enum AttackType { None, LightBlade, HeavyBlade };
    public enum MoveDirection { Forward,Right,Backward,Left };

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterAnimator))]
    // todo polymorphize to player and enemy combat system
    // todo also, is it possible to polymorphise animation controllers, otherwise seperate into player and enemy animation controllers
    public class CombatSystem : MonoBehaviour
    {

        #region Class Variables

        [SerializeField] CombatConfig config;

        public bool IsDamageFrame { get; private set; }

        public delegate void DamageFrameChanged(bool isDamageFrame);
        public event DamageFrameChanged OnDamageFrameChanged;
        public void CallDamageFrameChangedEvent(bool isDamageFrame)
        {
            OnDamageFrameChanged(isDamageFrame);
        }

        public AttackType CurrentAttackType {
            get
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.LIGHTATTACK_TAG))
                {
                    return AttackType.LightBlade;
                }
                else if (animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.HEAVYATTACK_TAG))
                {
                    return AttackType.HeavyBlade;
                }
                return AttackType.None;
            }
        }
        public float CurrentAttackDamage {
            get
            {
                switch (CurrentAttackType)
                {
                    case AttackType.LightBlade:
                        return config.LightAttackDamage;
                    case AttackType.HeavyBlade:
                        return config.HeavyAttackDamage;
                    default:
                        return 0;
                }
            }
        }
        private float resetAttackTriggerTime = 0;
        private bool runningResetCR = false;

        [HideInInspector] protected Animator animator;
        private AnimOverrideSetter animOverrideHandler;
        protected CharacterState characterState;
        private CombatSMB[] combatSMBs;

        #endregion

        protected virtual void Start()
        {
            characterState = GetComponent<CharacterState>();
            animator = GetComponent<Animator>();
            animator.SetFloat(AnimConstants.Parameters.ATTACK_SPEED_MULTIPLIER, config.AttackAnimSpeed);
            animOverrideHandler = GetComponent<AnimOverrideSetter>();
            combatSMBs = animator.GetBehaviours<CombatSMB>();

            foreach(CombatSMB smb in combatSMBs)
            {
                smb.AttackExitListeners += DamageEnd;
            }

            IsDamageFrame = false;
        }

        void OnDestroy()
        {
            foreach (CombatSMB smb in combatSMBs)
            {
                smb.AttackExitListeners -= DamageEnd;
            }
        }

        #region Attacks

        public void LightAttack()
        {
            animator.SetBool(AnimConstants.Parameters.ISHEAVY_BOOL, false);
            animator.SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
            resetAttackTriggerTime = Time.time + config.TimeToClearAttackTrigger;
            if(!runningResetCR) StartCoroutine(DelayedResetAttackTrigger());
        }

        public void HeavyAttack()
        {
            animator.SetBool(AnimConstants.Parameters.ISHEAVY_BOOL, true);
            animator.SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
            resetAttackTriggerTime = Time.time + config.TimeToClearAttackTrigger;
            if (!runningResetCR) StartCoroutine(DelayedResetAttackTrigger());
        }

        IEnumerator DelayedResetAttackTrigger()
        {
            runningResetCR = true;
            yield return new WaitWhile(() => Time.time < resetAttackTriggerTime);
            animator.ResetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
            runningResetCR = false;
        }

        #endregion

        #region Dodge

        public void Dodge(MoveDirection moveDirection = MoveDirection.Forward)
        {
            if (characterState.Uninteruptable || characterState.Dodging)
            {
                return;
            }

            AnimationClip animToUse;
            switch (moveDirection)
            {
                case MoveDirection.Right:
                    animToUse = config.DodgeRightAnimation;
                    break;
                case MoveDirection.Backward:
                    animToUse = config.DodgeBackwardAnimation;
                    break;
                case MoveDirection.Left:
                    animToUse = config.DodgeLeftAnimation;
                    break;
                default:
                    animToUse = config.DodgeForwardAnimation;
                    break;
            }

            enterDodgingState(animToUse);
        }

        private void enterDodgingState(AnimationClip animClip)
        {
            animOverrideHandler.SetTriggerOverride(AnimConstants.Parameters.DODGE_TRIGGER, AnimConstants.OverrideIndexes.DODGE_INDEX, animClip);
        }

        #endregion

        #region Combat Animation Events

        void DamageStart()
        {
            if (!IsDamageFrame)
            {
                CallDamageFrameChangedEvent(true);
                IsDamageFrame = true;
            }
        }

        void DamageEnd()
        {
            if (IsDamageFrame)
            {
                CallDamageFrameChangedEvent(false);
                IsDamageFrame = false;
            }
        }

        // todo make this and the class abstract when we add an enemy combat system
        public virtual void DealtDamage(HealthSystem target)
        {
            return;
        }

        #endregion

    }
}