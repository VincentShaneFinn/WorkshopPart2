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

        private float lightAttackDamage;
        private float heavyAttackDamage;


        [SerializeField] private float attackAnimSpeed = 1f;

        [HideInInspector] public Animator Animator;
        [HideInInspector] public CharacterAnimator CharacterAnim;

        void Start()
        {
            CharacterAnim = GetComponent<CharacterAnimator>();
            Animator = GetComponent<Animator>();
            Animator.SetFloat(AnimationParams.ATTACK_SPEED_MULTIPLIER, attackAnimSpeed);

            lightAttackDamage = config.LightAttackDamage;
            heavyAttackDamage = config.HeavyAttackDamage;
            IsDamageFrame = false;
        }

        void Update()
        {
            if (Animator.GetAnimatorTransitionInfo(0).anyState)
            {
                OnDamageFrameChanged(false);
            }

            // todo move to a central animatorStateHandler
            if (Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimationTags.ATTACK_TAG) &&
                !Animator.IsInTransition(0))
            {
                CharacterAnim.CanRotate = false;
                CharacterAnim.CanMove = false;
            }
            else
            {
                CharacterAnim.CanMove = true;
                CharacterAnim.CanRotate = true;
            }
        }

        public void LightAttack()
        {
            Animator.SetBool(AnimationParams.ISHEAVY_BOOL, false);
            Animator.SetTrigger(AnimationParams.ATTACK_TRIGGER);
            currentWeaponDamage = lightAttackDamage;
        }

        public void HeavyAttack()
        {
            Animator.SetBool(AnimationParams.ISHEAVY_BOOL, true);
            Animator.SetTrigger(AnimationParams.ATTACK_TRIGGER);
            currentWeaponDamage = heavyAttackDamage;
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
            CharacterAnim.animOverrideController[AnimationOverrideIndexes.DODGE_INDEX] = animClip;
            if (Animator.IsInTransition(0) ||
                Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationStates.KNOCKBACK_STATE) ||
                Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationStates.DODGE_STATE))
            {
                Animator.ResetTrigger(AnimationParams.DODGE_TRIGGER);
            }
            else
            {
                Animator.SetTrigger(AnimationParams.DODGE_TRIGGER);
            }
            Animator.ResetTrigger(AnimationParams.ATTACK_TRIGGER);
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