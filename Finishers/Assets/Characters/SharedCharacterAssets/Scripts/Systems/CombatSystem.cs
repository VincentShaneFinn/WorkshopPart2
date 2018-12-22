using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Finisher.Characters.Weapons;

namespace Finisher.Characters
{
    public enum MoveDirection { Forward,Right,Backward,Left };

    // todo this and the AnimatorStatehandler need to talk to each other better, if this is going to override it, as simple as being a suggestor at certain times 
    public class CombatSystem : MonoBehaviour
    {
        [SerializeField] CombatSystemConfig config;

        public float currentWeaponDamage { get; private set; }
        public bool IsDamageFrame { get; private set; }
        public delegate void DamageFrameChanged(bool isDamageFrame);
        public event DamageFrameChanged OnDamageFrameChanged;

        [SerializeField] private float attackAnimSpeed = 1f;
        private int nextAttackIndex = 0;

        private Animator animator;
        private CharacterAnimator characterAnim;

        void Start()
        {
            characterAnim = GetComponent<CharacterAnimator>();
            animator = GetComponent<Animator>();
            animator.SetFloat(AnimationParams.ATTACK_SPEED_MULTIPLIER, attackAnimSpeed);

            currentWeaponDamage = config.LightAttackDamage;
            IsDamageFrame = false;
        }

        void Update()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimationTags.ATTACK_TAG) &&
                !animator.IsInTransition(0))
            {
                characterAnim.CanRotate = false;
                characterAnim.CanMove = false;
            }
            else
            {
                characterAnim.CanMove = true;
                characterAnim.CanRotate = true;
            }
        }

        public void LightAttack()
        {
            animator.SetBool(AnimationParams.ISHEAVY_BOOL, false);
            animator.SetTrigger(AnimationParams.ATTACK_TRIGGER);
        }

        public void HeavyAttack()
        {
            animator.SetBool(AnimationParams.ISHEAVY_BOOL, true);
            animator.SetTrigger(AnimationParams.ATTACK_TRIGGER);
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
            //if (characterAnim.CanRotate)
            //{
                SetDodgeTrigger(config.DodgeForwardAnimation);
            //}
            //else
            //{
            //    SetDodgeTrigger(animToUse);
            //}
        }

        public void SetDodgeTrigger(AnimationClip animClip)
        {
            characterAnim.animOverrideController[AnimationOverrideIndexes.DODGE_INDEX] = animClip;
            if (animator.IsInTransition(0) ||
                animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationStates.KNOCKBACK_STATE) ||
                animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationStates.DODGE_STATE))
            {
                animator.ResetTrigger(AnimationParams.DODGE_TRIGGER);
                animator.ResetTrigger(AnimationParams.ATTACK_TRIGGER);
            }
            else
            {
                animator.SetTrigger(AnimationParams.DODGE_TRIGGER);
            }
        }

        // animation events

        // todo replace this with damageStart and end frames

        void Hit()
        {
            print("hit something now");
        }

        void DamageStart()
        {
            OnDamageFrameChanged(true);
        }

        void DamageEnd()
        {
            OnDamageFrameChanged(false);
        }

    }
}