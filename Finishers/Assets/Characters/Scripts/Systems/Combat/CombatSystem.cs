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

        public float LightAttackDamage { get { return config.LightAttackDamage; } }
        public float HeavyAttackDamage { get { return config.HeavyAttackDamage; } }
        public AttackType CurrentAttackType {
            get
            {
                if (Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.LIGHTATTACK_TAG))
                {
                    return AttackType.LightBlade;
                }
                else if (Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.HEAVYATTACK_TAG))
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
                        return LightAttackDamage;
                    case AttackType.HeavyBlade:
                        return HeavyAttackDamage;
                    default:
                        return 0;
                }
            }
        }
        private float resetAttackTriggerTime = 0;
        private bool runningResetCR = false;

        [SerializeField] private float timeToClearAttackInputQue = 0;
        [SerializeField] private float attackAnimSpeed = 1f;

        [HideInInspector] public Animator Animator;
        private CharacterAnimator character;
        private CombatSMB[] combatSMBs;

        #endregion

        protected virtual void Start()
        {
            character = GetComponent<CharacterAnimator>();
            Animator = GetComponent<Animator>();
            Animator.SetFloat(AnimConstants.Parameters.ATTACK_SPEED_MULTIPLIER, attackAnimSpeed);
            combatSMBs = Animator.GetBehaviours<CombatSMB>();

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
            Animator.SetBool(AnimConstants.Parameters.ISHEAVY_BOOL, false);
            Animator.SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
            resetAttackTriggerTime = Time.time + timeToClearAttackInputQue;
            if(!runningResetCR) StartCoroutine(DelayedResetAttackTrigger());
        }

        public void HeavyAttack()
        {
            Animator.SetBool(AnimConstants.Parameters.ISHEAVY_BOOL, true);
            Animator.SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
            resetAttackTriggerTime = Time.time + timeToClearAttackInputQue;
            if (!runningResetCR) StartCoroutine(DelayedResetAttackTrigger());
        }

        IEnumerator DelayedResetAttackTrigger()
        {
            runningResetCR = true;
            yield return new WaitWhile(() => Time.time < resetAttackTriggerTime);
            Animator.ResetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
            runningResetCR = false;
        }

        #endregion

        #region Dodge

        public void Dodge(MoveDirection moveDirection = MoveDirection.Forward)
        {
            if (character.Uninteruptable || Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.DODGE_STATE))
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
            SetDodgeTrigger(animToUse);
        }

        private void SetDodgeTrigger(AnimationClip animClip)
        {
            character.SetTriggerOverride(AnimConstants.Parameters.DODGE_TRIGGER, AnimConstants.OverrideIndexes.DODGE_INDEX, animClip);
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

        #endregion

    }
}