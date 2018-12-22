using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Finisher.Characters
{
    [RequireComponent(typeof(CharacterAnimator))]
    public class HealthSystem : MonoBehaviour
    {

        [SerializeField] Slider healthSlider;
        [SerializeField] HealthSystemConfig config;
        [SerializeField] float maxHealth = 100;

        private float currentHealth;

        private CharacterAnimator characterAnim;
        private Animator animator;
        private AnimatorOverrideController animOverrideController;

        void Start()
        {
            characterAnim = GetComponent<CharacterAnimator>();
            animOverrideController = characterAnim.animOverrideController;
            animator = GetComponent<Animator>();

            currentHealth = maxHealth;
            healthSlider.value = getCurrentHealthAsPercent();
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.K)) // todo remove testing buttons
            {
                Knockback(config.KnockbackAnimations[UnityEngine.Random.Range(0,config.KnockbackAnimations.Length)]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Damage(10);
            }
        }

        public void Damage(float damage)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationStates.DODGE_STATE)) { return; }
            currentHealth -= damage;
            healthSlider.value = getCurrentHealthAsPercent();
            Knockback(config.KnockbackAnimations[UnityEngine.Random.Range(0,config.KnockbackAnimations.Length)]);
            if(currentHealth <= 0)
            {
                Kill();
            }
        }

        public void Kill()
        {
            if (characterAnim.Dying) { return; }
            characterAnim.Dying = true;
            animator.SetBool(AnimationParams.DYING_BOOL, true);
        }

        public void Knockback(AnimationClip animClip)
        {
            animOverrideController[AnimationOverrideIndexes.KNOCKBACK_INDEX] = animClip;
            animator.SetTrigger(AnimationParams.KNOCKBACK_TRIGGER);
        }

        private float getCurrentHealthAsPercent()
        {
            return currentHealth / maxHealth;
        }
    }
}
