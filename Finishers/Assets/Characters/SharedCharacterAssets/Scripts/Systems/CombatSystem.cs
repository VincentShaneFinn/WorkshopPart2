using System.Collections;
using UnityEngine;

namespace Finisher.Characters
{
    public enum MoveDirection { Forward,Right,Backward,Left };

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterAnimator))]
    public class CombatSystem : MonoBehaviour
    {
        [SerializeField] CombatSystemConfig config;

        public float currentWeaponDamage { get; private set; }
        public bool IsDamageFrame { get; private set; }
        public bool IsAttacking
        {
            get
            {
                return Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.ATTACKRIGHT_TAG) ||
                    Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.ATTACKLEFT_TAG);
            }
        }
        public bool IsDodging {
            get
            {
                return Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimContstants.States.DODGE_STATE);
            }
        }
        public delegate void DamageFrameChanged(bool isDamageFrame);
        public event DamageFrameChanged OnDamageFrameChanged;

        private float lightAttackDamage;
        private float heavyAttackDamage;
        private float resetAttackTriggerTime = 0;
        private bool runningResetCR = false;

        [SerializeField] private float timeToClearAttackInputQue = 0;
        [SerializeField] private float attackAnimSpeed = 1f;

        [HideInInspector] public Animator Animator;
        [HideInInspector] public CharacterAnimator CharacterAnim;
        private CombatSMB[] combatSMBs;

        void Start()
        {
            CharacterAnim = GetComponent<CharacterAnimator>();
            Animator = GetComponent<Animator>();
            Animator.SetFloat(AnimContstants.Parameters.ATTACK_SPEED_MULTIPLIER, attackAnimSpeed);
            combatSMBs = Animator.GetBehaviours<CombatSMB>();
            foreach(CombatSMB smb in combatSMBs)
            {
                smb.AttackExitListeners += DamageEnd;
            }

            lightAttackDamage = config.LightAttackDamage;
            heavyAttackDamage = config.HeavyAttackDamage;
            IsDamageFrame = false;
        }

        void Update()
        {
            LockMovementInCombatAction();

        }

        private void LockMovementInCombatAction()
        {
            if (!Animator.IsInTransition(0))
            {
                if (IsAttacking ||
                    Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.UNINTERUPTABLE_TAG))
                {
                    CharacterAnim.CanRotate = false;
                    CharacterAnim.CanMove = false;
                }
            }
            else
            {
                CharacterAnim.CanMove = true;
                CharacterAnim.CanRotate = true;
            }
        }

        public void LightAttack()
        {
            Animator.runtimeAnimatorController = CharacterAnim.animOverrideController;
            Animator.SetBool(AnimContstants.Parameters.ISHEAVY_BOOL, false);
            Animator.SetTrigger(AnimContstants.Parameters.ATTACK_TRIGGER);
            currentWeaponDamage = lightAttackDamage;
            resetAttackTriggerTime = Time.time + timeToClearAttackInputQue;
            if(!runningResetCR) StartCoroutine(DelayedResetAttackTrigger());
        }

        public void HeavyAttack()
        {
            Animator.runtimeAnimatorController = CharacterAnim.animOverrideController;
            Animator.SetBool(AnimContstants.Parameters.ISHEAVY_BOOL, true);
            Animator.SetTrigger(AnimContstants.Parameters.ATTACK_TRIGGER);
            currentWeaponDamage = heavyAttackDamage;
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
            Animator.runtimeAnimatorController = CharacterAnim.animOverrideController;
            SetDodgeTrigger(animToUse);
        }

        public void SetDodgeTrigger(AnimationClip animClip)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.UNINTERUPTABLE_TAG) || 
                Animator.GetAnimatorTransitionInfo(0).anyState)
            {
                Animator.ResetTrigger(AnimContstants.Parameters.DODGE_TRIGGER);
            }
            else
            {
                CharacterAnim.animOverrideController[AnimContstants.OverrideIndexes.DODGE_INDEX] = animClip;
                Animator.SetTrigger(AnimContstants.Parameters.DODGE_TRIGGER);
            }
        }

        #region Combat Animation Events

        void Hit()
        {
            print("hit something now");
        }

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