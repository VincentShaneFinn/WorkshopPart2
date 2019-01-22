using System.Collections;
using UnityEngine;

using Finisher.Characters.Systems.Strategies;
using System;

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
        [SerializeField] protected CombatConfig config;

        public bool IsDamageFrame { get; private set; }
        public bool DodgingAllowed = true; 

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
        public void CallCombatSystemDealtDamageListeners(float amount)
        {
            if (OnHitEnemy != null)
            {
                OnHitEnemy(amount);
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
        private int hitCounter;

        [HideInInspector] protected Animator animator;
        private AnimOverrideSetter animOverrideHandler;
        protected CharacterState characterState;
        private CombatSMB[] combatSMBs;
        private DodgeSMB[] dodgeSMBs;
        private ParrySMB[] parrySMBs;
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
            parrySMBs = animator.GetBehaviours<ParrySMB>();
            finisherSystem = GetComponent<FinisherSystem>();

            foreach(CombatSMB smb in combatSMBs)
            {
                smb.AttackExitListeners += DamageEnd;
                smb.AttackExitListeners += RestoreDodging;
                smb.AttackStartListeners += attemptRiposte;
            }

            foreach (DodgeSMB smb in dodgeSMBs)
            {
                smb.DodgeExitListeners += DodgeEnd;
            }

            foreach (ParrySMB smb in parrySMBs)
            {
                smb.ParryExitListeners += ParryEnd;
            }

            HealthSystem healthSystem = GetComponent<HealthSystem>();

            if (healthSystem)
            {
                healthSystem.OnDamageTaken += resetHitCounter;
            }

            IsDamageFrame = false;
        }

        void OnDestroy()
        {
            foreach (CombatSMB smb in combatSMBs)
            {
                smb.AttackExitListeners -= DamageEnd;
                smb.AttackExitListeners -= RestoreDodging;
                smb.AttackStartListeners -= attemptRiposte;
            }

            foreach (DodgeSMB smb in dodgeSMBs)
            {
                smb.DodgeExitListeners -= DodgeEnd;
            }

            HealthSystem healthSystem = GetComponent<HealthSystem>();

            if (healthSystem)
            {
                healthSystem.OnDamageTaken -= resetHitCounter;
            }

            foreach (ParrySMB smb in parrySMBs)
            {
                smb.ParryExitListeners += ParryEnd;
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
            if (characterState.Uninteruptable || characterState.Dodging  || !DodgingAllowed)
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

        #region Parry

        public void Parry()
        {
            if (characterState.Uninteruptable || characterState.Parrying)
            {
                return;
            }

            animator.SetTrigger(AnimConstants.Parameters.PARRY_TRIGGER);
        }

        protected virtual void attemptRiposte()
        {
            //TODO: Make abstract and implement in enemy
        }

        #endregion

        #region Combat Animation Events

        #region Damage

        void DamageStart()
        {
            if (!IsDamageFrame)
            {
                CallDamageFrameChangedEvent(true);
                IsDamageFrame = true;
            }

            DodgingAllowed = false;
        }

        void RestoreDodging()
        {
            DodgingAllowed = true;
        }

        void DamageEnd()
        {
            if (IsDamageFrame)
            {
                CallDamageFrameChangedEvent(false);
                IsDamageFrame = false;
            }
        }

        #endregion

        #region Dodge

        void DodgeStart()
        {
            characterState.IsDodgeFrame = true;
        }

        void DodgeEnd()
        {
            characterState.IsDodgeFrame = false;
        }

        #endregion

        #region Parry

        void ParryStart()
        {
            characterState.IsParryFrame = true;
        }

        void ParryEnd()
        {
            characterState.IsParryFrame = false;
        }

        #endregion

        // todo make this and the class abstract when we add an enemy combat system
        public virtual void HitCharacter(HealthSystem targetHealthSystem)
        {
            if (CurrentAttackType == AttackType.LightBlade)
            {
                float finisherMeterGain = lightAttackDamageSystem.FinisherMeterGainAmount;

                finisherMeterGain = multiplyFinisherMeterGain(finisherMeterGain);
                
                lightAttackDamageSystem.HitCharacter(gameObject, targetHealthSystem);
                CallCombatSystemDealtDamageListeners(finisherMeterGain);
            }
            else if (CurrentAttackType == AttackType.HeavyBlade)
            {
                float finisherMeterGain = heavyAttackDamageSystem.FinisherMeterGainAmount;

                finisherMeterGain = multiplyFinisherMeterGain(finisherMeterGain);

                heavyAttackDamageSystem.HitCharacter(gameObject, targetHealthSystem);
                CallCombatSystemDealtDamageListeners(finisherMeterGain);
            }

            if (gameObject.tag == "Player")
            {
                hitCounter++;
                hitCounter = Mathf.Clamp(hitCounter, 0, 15);
            }
        }

        private float multiplyFinisherMeterGain(float finisherMeterGain)
        {
            if (hitCounter > 5)
            {
                finisherMeterGain = finisherMeterGain * (1 + (.05f * (hitCounter - 5)));
            }
            
            return finisherMeterGain;
        }

        private void resetHitCounter()
        {
            hitCounter = 0;
        }

        #endregion

    }
}