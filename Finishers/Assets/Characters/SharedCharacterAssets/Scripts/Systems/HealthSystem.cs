using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Finisher.Characters
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterAnimator))]
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

            getPlayerHealthSlider();

            currentHealth = maxHealth;
            healthSlider.value = GetHealthAsPercent();

            if (volatilityMeter)
            {
                decreaseVolatility(maxVolatility);
            }
        }

        private void getPlayerHealthSlider()
        {
            if(gameObject.tag == "Player")
            {
                healthSlider = FindObjectOfType<UI.PlayerUIObjects>().HealthSlider;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                DamageHealth(10);
            }
        }

        #region Public Interface

        #region Change Health

        public void DamageHealth(float damage)
        {
            //Dont deal damage if dodging
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimContstants.States.DODGE_STATE)) { return; }

            decreaseHealth(damage);
            attempKnockback();
            checkToKill();
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

        private void checkToKill()
        {
            if (currentHealth <= 0)
            {
                Kill(config.NormalDeathAnimations[UnityEngine.Random.Range(0, config.NormalDeathAnimations.Length)]);
            }
        }

        private void increaseHealth(float healing)
        {
            currentHealth += healing;
            if(currentHealth > maxHealth - Mathf.Epsilon)
            {
                currentHealth = maxHealth;
            }
            healthSlider.value = GetHealthAsPercent();
        }

        private void decreaseHealth(float damage)
        {
            currentHealth -= damage;
            if(currentHealth < Mathf.Epsilon)
            {
                currentHealth = 0;
            }
            healthSlider.value = GetHealthAsPercent();
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
            volatilityMeter.value = GetVolaitilityAsPercent();
        }

        private void decreaseVolatility(float amount)
        {
            currentVolatility -= amount;
            if(currentVolatility <= Mathf.Epsilon)
            {
                currentVolatility = 0;
            }
            volatilityMeter.value = GetVolaitilityAsPercent();
        }

        public float GetVolaitilityAsPercent()
        {
            return currentVolatility / maxVolatility;
        }

        private void checkVolatilityFull()
        {
            if (currentVolatility >= maxVolatility)
            {
                print("Volatility Full");
                character.Staggered = true; // todo protect from leaving grab mode?
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
            character.SetTriggerOverride(AnimContstants.Parameters.KNOCKBACK_TRIGGER, AnimContstants.OverrideIndexes.KNOCKBACK_INDEX, animClip);
        }

        public void Kill(AnimationClip animClip)
        {
            if (character.Dying) { return; }
            currentHealth = 0;
            healthSlider.value = GetHealthAsPercent();
            character.Dying = true;
            character.SetBoolOverride(AnimContstants.Parameters.DYING_BOOL, true, AnimContstants.OverrideIndexes.DEATH_INDEX, animClip);
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
    }
}
