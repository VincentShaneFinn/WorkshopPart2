using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Finisher.Characters.Systems
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterAnimator))]
    public abstract class HealthSystem : MonoBehaviour
    {
        [SerializeField] protected HealthConfig config;

        protected float currentHealth { get; set; }
        protected float currentVolatility { get; set; }
        protected int knockbackCount;

        #region Delegates

        public delegate void KnockedBack();
        public event KnockedBack OnKnockBack;
        private void CallKnockbackEvent()
        {
            if (OnKnockBack != null)
            {
                OnKnockBack();
            }
        }

        #endregion

        private CharacterState characterState;
        private AnimOverrideSetter animOverrideHandler;
        protected Slider healthSlider;

        protected virtual void Start()
        {
            characterState = GetComponent<CharacterState>();
            animOverrideHandler = GetComponent<AnimOverrideSetter>();

            increaseHealth(config.MaxHealth);
            decreaseVolatility(config.MaxVolatility);
        }

        #region Public Interface

        #region Change Health

        public void DamageHealth(float damage)
        {
            //Dont deal damage if dodging
            if (characterState.Invulnerable) { return; }

            decreaseHealth(damage);

            if (knockbackCount < config.KnockbackLimit)
            {
                Knockback();
            }

            if (currentHealth <= 0)
            {
                Kill();
            }
        }

        private void increaseHealth(float healing)
        {
            currentHealth += healing;
            if(currentHealth > config.MaxHealth - Mathf.Epsilon)
            {
                currentHealth = config.MaxHealth;
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
            return currentHealth / config.MaxHealth;
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
            if (currentVolatility > config.MaxVolatility - Mathf.Epsilon)
            {
                currentVolatility = config.MaxVolatility;
            }
            updateVolatilityUI();
        }

        private void decreaseVolatility(float amount)
        {
            currentVolatility -= amount;
            if (currentVolatility <= Mathf.Epsilon)
            {
                currentVolatility = 0;
            }
            updateVolatilityUI();
        }

        public float GetVolaitilityAsPercent()
        {
            return currentVolatility / config.MaxVolatility;
        }

        private void checkVolatilityFull()
        {
            if (currentVolatility >= config.MaxVolatility)
            {
                //character.Staggered = true; // todo if stagger implemented here, protect from leaving grab mode
            }
        }

        #endregion

        // todo knockback is currently really a stagger, and we need to add a knockback with a movement vector
        #region Knockback And Kill

        public void Knockback()
        {
            Knockback(config.KnockbackAnimations[UnityEngine.Random.Range(0, config.KnockbackAnimations.Length)]);
        }
        public void Knockback(AnimationClip animClip)
        {
            if (characterState.Dying) { return; }

            enterKnockbackState(animClip);
            CallKnockbackEvent();

            knockbackCount++;
            StartCoroutine(releaseCountAfterDelay());
        }

        private void enterKnockbackState(AnimationClip animClip)
        {
            animOverrideHandler.SetTriggerOverride(AnimConstants.Parameters.KNOCKBACK_TRIGGER, AnimConstants.OverrideIndexes.KNOCKBACK_INDEX, animClip);
        }

        // todo, make this care about consective hits or building up a resistance?
        private IEnumerator releaseCountAfterDelay()
        {
            yield return new WaitForSeconds(config.FreeKnockbackTime);
            knockbackCount--;
        }

        public void Kill()
        {
            Kill(config.NormalDeathAnimations[UnityEngine.Random.Range(0, config.NormalDeathAnimations.Length)]);
        }
        public virtual void Kill(AnimationClip animClip)
        {
            if (characterState.Dying) { return; }
            currentHealth = 0;
            updateHealthUI();
            enterDyingState(animClip);
        }

        private void enterDyingState(AnimationClip animClip)
        {
            characterState.DyingState.Kill();
            animOverrideHandler.SetBoolOverride(AnimConstants.Parameters.DYING_BOOL, true,AnimConstants.OverrideIndexes.DEATH_INDEX, animClip);
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

        #endregion

        protected abstract void updateVolatilityUI();

    }
}
