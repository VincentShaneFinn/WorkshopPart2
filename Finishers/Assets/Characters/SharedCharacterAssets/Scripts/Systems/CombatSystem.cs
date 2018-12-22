﻿using UnityEngine;

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
            Animator.SetFloat(AnimContstants.Parameters.ATTACK_SPEED_MULTIPLIER, attackAnimSpeed);

            lightAttackDamage = config.LightAttackDamage;
            heavyAttackDamage = config.HeavyAttackDamage;
            IsDamageFrame = false;
        }

        void Update()
        {
            // todo find a better way to catch when an attack is interupped, and rename Animation States
            if (Animator.GetAnimatorTransitionInfo(0).anyState || Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimContstants.States.KNOCKBACK_STATE))
            {
                OnDamageFrameChanged(false);
            }

            // todo move to a central animatorStateHandler
            if (Animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.ATTACK_TAG) &&
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
            Animator.SetBool(AnimContstants.Parameters.ISHEAVY_BOOL, false);
            Animator.SetTrigger(AnimContstants.Parameters.ATTACK_TRIGGER);
            currentWeaponDamage = lightAttackDamage;
        }

        public void HeavyAttack()
        {
            Animator.SetBool(AnimContstants.Parameters.ISHEAVY_BOOL, true);
            Animator.SetTrigger(AnimContstants.Parameters.ATTACK_TRIGGER);
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
            CharacterAnim.animOverrideController[AnimContstants.OverrideIndexes.DODGE_INDEX] = animClip;
            if (Animator.IsInTransition(0) ||
                Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimContstants.States.KNOCKBACK_STATE) ||
                Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimContstants.States.DODGE_STATE))
            {
                Animator.ResetTrigger(AnimContstants.Parameters.DODGE_TRIGGER);
            }
            else
            {
                Animator.SetTrigger(AnimContstants.Parameters.DODGE_TRIGGER);
            }
            Animator.ResetTrigger(AnimContstants.Parameters.ATTACK_TRIGGER);
        }

        #region Combat Animation Events

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

        #endregion

    }
}