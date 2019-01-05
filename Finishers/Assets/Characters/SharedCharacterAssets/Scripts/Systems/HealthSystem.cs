﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Finisher.Characters
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterAnimator))]
    // todo polymorphise To Player and Enemy Health System
    public class HealthSystem : MonoBehaviour
    {
        [Tooltip("Player's is set automatically")]
        [SerializeField] Slider healthSlider;
        [SerializeField] Slider volatilityMeter;
        [SerializeField] HealthSystemConfig config;
        [SerializeField] float maxHealth = 100f;
        [SerializeField] float maxVolatility = 100f;
        [SerializeField] int KnockbackLimit = 2;
        [SerializeField] float FreeKnockbackTime = 1f;

        private float currentHealth;
        private float currentVolatility;
        private int knockbackCount;

        [HideInInspector] public CharacterAnimator character;
        [HideInInspector] public Animator Animator;

        void Start()
        {
            character = GetComponent<CharacterAnimator>();

            Animator = GetComponent<Animator>();

            setPlayerHealthSlider();
            increaseHealth(maxHealth);
            decreaseVolatility(maxVolatility);
            setupVolatilityMeter();
        }

        private void setPlayerHealthSlider()
        {
            if(gameObject.tag == "Player")
            {
                healthSlider = FindObjectOfType<UI.PlayerUIObjects>().HealthSlider;
            }
        }

        private void setupVolatilityMeter()
        {
            if (volatilityMeter)
            {
                volatilityMeter.gameObject.SetActive(false);
                FinisherSystem playerFinisherSystem = GameObject.FindGameObjectWithTag(TagNames.PlayerTag).GetComponent<FinisherSystem>();
                if (playerFinisherSystem)
                {
                    playerFinisherSystem.OnFinisherModeToggled += toggleVolatiltyMeter;
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                DamageHealth(10);
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                character.Stunned = !character.Stunned;
            }
        }

        #region Public Interface

        #region Change Health

        public void DamageHealth(float damage)
        {
            //Dont deal damage if dodging
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.DODGE_STATE)) { return; }

            decreaseHealth(damage);
            attempKnockback();

            if (currentHealth <= 0)
            {
                Kill();
            }
        }

        private void attempKnockback()
        {
            if (knockbackCount < KnockbackLimit)
            {
                // todo make a unique random number generator so they dont all die the same way each frame?
                Knockback(config.KnockbackAnimations[UnityEngine.Random.Range(0, config.KnockbackAnimations.Length)]);
                knockbackCount++;
                StartCoroutine(releaseCountAfterDelay());
            }
        }

        private void increaseHealth(float healing)
        {
            currentHealth += healing;
            if(currentHealth > maxHealth - Mathf.Epsilon)
            {
                currentHealth = maxHealth;
            }
            updateHealthUI();
        }

        private void decreaseHealth(float damage)
        {
            currentHealth -= damage;
            if(currentHealth < Mathf.Epsilon)
            {
                currentHealth = 0;
            }
            updateHealthUI();
        }

        public float GetHealthAsPercent()
        {
            return currentHealth / maxHealth;
        }

        #endregion

        #region Change Volatility

        public void DamageVolatility(float amount)
        {
            increaseVolatility(amount);
            checkVolatilityFull();
        }

        private void increaseVolatility(float amount)
        {
            currentVolatility += amount;
            if(currentVolatility > maxVolatility - Mathf.Epsilon)
            {
                currentVolatility = maxVolatility;
            }
            updateVolatilityUI();
        }

        private void decreaseVolatility(float amount)
        {
            currentVolatility -= amount;
            if(currentVolatility <= Mathf.Epsilon)
            {
                currentVolatility = 0;
            }
            updateVolatilityUI();
        }

        public float GetVolaitilityAsPercent()
        {
            return currentVolatility / maxVolatility;
        }

        private void checkVolatilityFull()
        {
            if (currentVolatility >= maxVolatility)
            {
                //character.Staggered = true; // todo protect from leaving grab mode?
            }
        }

        #endregion

        #region Knockback And Kill

        // todo, make this care about consective hits or building up a resistance?
        private IEnumerator releaseCountAfterDelay()
        {
            yield return new WaitForSeconds(FreeKnockbackTime);
            knockbackCount--;
        }

        public void Knockback(AnimationClip animClip)
        {
            if (!character.Uninteruptable || 
                Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.KNOCKBACK_STATE)||
                Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.STUNNED_STATE))
            {
                character.SetTriggerOverride(AnimConstants.Parameters.KNOCKBACK_TRIGGER, AnimConstants.OverrideIndexes.KNOCKBACK_INDEX, animClip);
            }
        }

        public void Kill()
        {
            Kill(config.NormalDeathAnimations[UnityEngine.Random.Range(0, config.NormalDeathAnimations.Length)]);
        }
        public void Kill(AnimationClip animClip)
        {
            if (character.Dying) { return; }
            currentHealth = 0;
            updateHealthUI();
            character.Dying = true;
            character.SetBoolOverride(AnimConstants.Parameters.DYING_BOOL, true, AnimConstants.OverrideIndexes.DEATH_INDEX, animClip);
        }

        #endregion

        public bool WillDamageKill(float damage)
        {
            if(currentHealth - damage <= Mathf.Epsilon)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region updateUI

        private void updateHealthUI()
        {
            if (healthSlider)
            {
                healthSlider.value = GetHealthAsPercent();
            }
        }

        private void updateVolatilityUI()
        {
            if (volatilityMeter)
            {
                volatilityMeter.value = GetVolaitilityAsPercent();
            }
        }

        private void toggleVolatiltyMeter(bool enabled)
        {
            currentVolatility = 0;
            updateVolatilityUI();

            volatilityMeter.gameObject.SetActive(enabled);
        }

        #endregion
    }
}
