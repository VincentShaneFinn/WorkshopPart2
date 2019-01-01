using System.Collections;
using UnityEngine;

namespace Finisher.Characters
{

    public enum AttackType { None, LightBlade, HeavyBlade };
    public enum MoveDirection { Forward,Right,Backward,Left };

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterAnimator))]
    public class CombatSystem : MonoBehaviour
    {

        #region Class Variables

        [SerializeField] CombatSystemConfig config;

        public bool IsDamageFrame { get; private set; }
        public delegate void DamageFrameChanged(bool isDamageFrame);
        public event DamageFrameChanged OnDamageFrameChanged;

        public float LightAttackDamage { get; private set; }
        public float HeavyAttackDamage { get; private set; }
        public AttackType CurrentAttackType {
            get
            {
                if (Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.LIGHTATTACK_TAG))
                {
                    return AttackType.LightBlade;
                }
                else if (Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.HEAVYATTACK_TAG))
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

        void Start()
        {
            character = GetComponent<CharacterAnimator>();
            Animator = GetComponent<Animator>();
            Animator.SetFloat(AnimContstants.Parameters.ATTACK_SPEED_MULTIPLIER, attackAnimSpeed);
            combatSMBs = Animator.GetBehaviours<CombatSMB>();
            foreach(CombatSMB smb in combatSMBs)
            {
                smb.AttackExitListeners += DamageEnd;
            }

            LightAttackDamage = config.LightAttackDamage;
            HeavyAttackDamage = config.HeavyAttackDamage;
            IsDamageFrame = false;
        }

        #region Attacks

        public void LightAttack()
        {
            Animator.SetBool(AnimContstants.Parameters.ISHEAVY_BOOL, false);
            Animator.SetTrigger(AnimContstants.Parameters.ATTACK_TRIGGER);
            resetAttackTriggerTime = Time.time + timeToClearAttackInputQue;
            if(!runningResetCR) StartCoroutine(DelayedResetAttackTrigger());
        }

        public void HeavyAttack()
        {
            Animator.SetBool(AnimContstants.Parameters.ISHEAVY_BOOL, true);
            Animator.SetTrigger(AnimContstants.Parameters.ATTACK_TRIGGER);
            resetAttackTriggerTime = Time.time + timeToClearAttackInputQue;
            if (!runningResetCR) StartCoroutine(DelayedResetAttackTrigger());
        }

        IEnumerator DelayedResetAttackTrigger()
        {
            runningResetCR = true;
            yield return new WaitWhile(() => Time.time < resetAttackTriggerTime);
            Animator.ResetTrigger(AnimContstants.Parameters.ATTACK_TRIGGER);
            runningResetCR = false;
        }

        #endregion

        #region Dodge

        public void Dodge(MoveDirection moveDirection = MoveDirection.Forward)
        {
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

        public void SetDodgeTrigger(AnimationClip animClip)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.UNINTERUPTABLE_TAG) || 
                Animator.GetAnimatorTransitionInfo(0).anyState ||
                character.Staggered ||
                character.Grabbing)
            {
                Animator.ResetTrigger(AnimContstants.Parameters.DODGE_TRIGGER);
            }
            else
            {
                character.SetTriggerOverride(AnimContstants.Parameters.DODGE_TRIGGER, AnimContstants.OverrideIndexes.DODGE_INDEX, animClip);
            }
        }

        #endregion

        #region Combat Animation Events

        void DamageStart()
        {
            if (!IsDamageFrame)
            {
                OnDamageFrameChanged(true);
                IsDamageFrame = true;
            }
        }

        void DamageEnd()
        {
            if (IsDamageFrame)
            {
                OnDamageFrameChanged(false);
                IsDamageFrame = false;
            }
        }

        #endregion

    }
}