using System.Collections;
using UnityEngine;

using Finisher.Characters.Systems.Strategies;

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

        [SerializeField] private CoreCombatDamageSystem lightAttackDamageSystem;
        [SerializeField] private CoreCombatDamageSystem heavyAttackDamageSystem;
        [SerializeField] CombatConfig config;

        public bool IsDamageFrame { get; private set; }
        public bool dodgingAllowed

        #region Delegates

        public delegate void DamageFrameChanged(bool isDamageFrame);
        public event DamageFrameChanged OnDamageFrameChanged;
        public void CallDamageFrameChangedEvent(bool isDamageFrame)
        {
            if (OnDamageFrameChanged != null)
            {
                OnDamageFrameChanged(isDamageFrame);
            }
        }

        public delegate void HitEnemyDelegate(float amount);
        public event HitEnemyDelegate OnHitEnemy;
        private void CallCombatSystemDealtDamageListeners(CoreCombatDamageSystem coreCombatDamageSystem)
        {
            if (OnHitEnemy != null)
            {
                OnHitEnemy(coreCombatDamageSystem.FinisherMeterGainAmount);
            }
        }

        #endregion

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

        private float resetAttackTriggerTime = 0;
        private bool runningResetCR = false;

        [HideInInspector] protected Animator animator;
        private AnimOverrideSetter animOverrideHandler;
        protected CharacterState characterState;
        private CombatSMB[] combatSMBs;
        private DodgeSMB[] dodgeSMBs;
        protected FinisherSystem finisherSystem;

        #endregion

        protected virtual void Start()
        {
            characterState = GetComponent<CharacterState>();
            animator = GetComponent<Animator>();
            animator.SetFloat(AnimConstants.Parameters.ATTACK_SPEED_MULTIPLIER, config.AttackAnimSpeed);
            animOverrideHandler = GetComponent<AnimOverrideSetter>();
            combatSMBs = animator.GetBehaviours<CombatSMB>();
            dodgeSMBs = animator.GetBehaviours<DodgeSMB>();
            finisherSystem = GetComponent<FinisherSystem>();

            foreach(CombatSMB smb in combatSMBs)
            {
                smb.AttackExitListeners += DamageEnd;
            }

            foreach (DodgeSMB smb in dodgeSMBs)
            {
                smb.DodgeExitListeners += DodgeEnd;
            }

            IsDamageFrame = false;
        }

        void OnDestroy()
        {
            foreach (CombatSMB smb in combatSMBs)
            {
                smb.AttackExitListeners -= DamageEnd;
            }

            foreach (DodgeSMB smb in dodgeSMBs)
            {
                smb.DodgeExitListeners -= DodgeEnd;
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

        void DodgeStart()
        {
            characterState.IsDodgeFrame = true;
        }

        void DodgeEnd()
        {
            characterState.IsDodgeFrame = false;
        }

        // todo make this and the class abstract when we add an enemy combat system
        public virtual void HitCharacter(HealthSystem targetHealthSystem)
        {
            if (CurrentAttackType == AttackType.LightBlade)
            {
                lightAttackDamageSystem.HitCharacter(targetHealthSystem);
                CallCombatSystemDealtDamageListeners(lightAttackDamageSystem);
            }
            else if (CurrentAttackType == AttackType.HeavyBlade)
            {
                heavyAttackDamageSystem.HitCharacter(targetHealthSystem);
                CallCombatSystemDealtDamageListeners(heavyAttackDamageSystem);
            }
        }

        #endregion

    }
}