using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Finisher.Characters
{
    public enum MoveDirection { Forward,Right,Backward,Left };

    // todo this and the AnimatorStatehandler need to talk to each other better, if this is going to override it, as simple as being a suggestor at certain times 
    public class CombatSystem : MonoBehaviour
    {
        [SerializeField] CombatSystemConfig config;

        public float currentWeaponDamage { get; private set; }

        private Animator animator;
        private CharacterAnimator characterAnim;

        private int nextAttackIndex = 0;

        void Start()
        {
            characterAnim = GetComponent<CharacterAnimator>();
            animator = GetComponent<Animator>();

            currentWeaponDamage = config.LightAttackDamage;
 
        }

        public void LightAttack()
        {
            animator.SetBool(CharAnimParams.ISHEAVY_BOOL, false);
            animator.SetTrigger(CharAnimParams.ATTACK_TRIGGER);
        }

        public void HeavyAttack()
        {
            animator.SetBool(CharAnimParams.ISHEAVY_BOOL, true);
            animator.SetTrigger(CharAnimParams.ATTACK_TRIGGER);
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
            if (characterAnim.CanRotate)
            {
                SetDodgeTrigger(config.DodgeForwardAnimation);
            }
            else
            {
                SetDodgeTrigger(animToUse);
            }
        }

        public void SetDodgeTrigger(AnimationClip animClip)
        {
            characterAnim.animOverrideController[AnimOverrideIndexes.DODGE_INDEX] = animClip;
            if (animator.IsInTransition(0) ||
                animator.GetCurrentAnimatorStateInfo(0).IsName(CharAnimStates.KNOCKBACK_STATE) ||
                animator.GetCurrentAnimatorStateInfo(0).IsName(CharAnimStates.DODGE_STATE))
            {
                animator.ResetTrigger(CharAnimParams.DODGE_TRIGGER);
                animator.ResetTrigger(CharAnimParams.ATTACK_TRIGGER);
            }
            else
            {
                animator.SetTrigger(CharAnimParams.DODGE_TRIGGER);
            }
        }

        // animation events

        public void Hit()
        {
            print("hit something now");
        }

    }
}