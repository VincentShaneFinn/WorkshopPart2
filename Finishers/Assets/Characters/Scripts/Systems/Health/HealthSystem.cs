using System.Collections;
using UnityEngine;

using Finisher.UI.Meters;

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
        public delegate void TookDamage();
        public event TookDamage OnDamageTaken;
        private void CallDamageTakenEvent()
        {
            if (OnDamageTaken != null)
            {
                OnDamageTaken();
            }
        }

        #endregion

        private CharacterState characterState;
        private AnimOverrideSetter animOverrideHandler;
        protected UI_HealthMeter healthBar;

        protected virtual void Start()
        {
            characterState = GetComponent<CharacterState>();
            animOverrideHandler = GetComponent<AnimOverrideSetter>();

            IncreaseHealth(config.MaxHealth);
            decreaseVolatility(config.MaxVolatility);
        }

        #region Public Interface

        #region Change Health

        public void DamageHealth(float damage)
        {
            //Dont deal damage if dodging
            if (characterState.Invulnerable) { return; }

            decreaseHealth(damage);
            CallDamageTakenEvent();

            if (currentHealth <= 0)
            {
                Kill();
            }
        }

        public void IncreaseHealth(float healing)
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
            if (characterState.Grabbed)
            {
                currentVolatility += amount * 3;
            }
            else
            {
                currentVolatility += amount;
            }

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

        public void Knockback(Vector3 knockbackVector, float knockbackTime = 0.1f, AnimationClip animClip = null)
        {
            //TODO: Add a method to override the knockback limiter
            if (characterState.Dying || knockbackCount >= config.KnockbackLimit) { return; }

            if (animClip == null)
            {
                animClip = config.KnockbackAnimations[UnityEngine.Random.Range(0, config.KnockbackAnimations.Length)];
            }

            enterKnockbackState(animClip);
            CallKnockbackEvent();

            knockbackCount++;

            StartCoroutine(releaseCountAfterDelay());

            // Transform side of knockback
            Vector3 knockbackTarget = transform.position + knockbackVector;
            IEnumerator coroutine = knockbackTowards(knockbackTarget, knockbackTime);
            StartCoroutine(coroutine);
        }

        public void KnockbackOutwards(GameObject damageSource, float knockbackRange, float knockbackTime = 0.1f, AnimationClip animClip = null)
        {
            Knockback((Vector3.Normalize(transform.position - damageSource.transform.position) * knockbackRange), knockbackTime, animClip);
        }

        public void Knockback(AnimationClip animClip = null)
        {
            //TODO: Add a method to override the knockback limiter
            if (characterState.Dying || knockbackCount >= config.KnockbackLimit) { return; }

            if(animClip == null)
            {
                animClip = config.KnockbackAnimations[UnityEngine.Random.Range(0, config.KnockbackAnimations.Length)];
            }

            enterKnockbackState(animClip);
            CallKnockbackEvent();

            knockbackCount++;
            StartCoroutine(releaseCountAfterDelay());
        }

        private void enterKnockbackState(AnimationClip animClip)
        {
            animOverrideHandler.SetTriggerOverride(AnimConstants.Parameters.KNOCKBACK_TRIGGER, AnimConstants.OverrideIndexes.KNOCKBACK_INDEX, animClip);
        }

        private IEnumerator knockbackTowards(Vector3 knockbackTarget, float knockbackTime)
        {
            float delay = 0.01f;
            float moveTimes = knockbackTime / delay;
            float maxDistance = Vector3.Distance(transform.position, knockbackTarget) / moveTimes;
            for (int i = 0; i < moveTimes; i++)
            {
                transform.position = Vector3.MoveTowards(transform.position, knockbackTarget, maxDistance);
                yield return new WaitForSeconds(delay);
            }
        }

        // todo, make this care about consective hits or building up a resistance?
        private IEnumerator releaseCountAfterDelay()
        {
            yield return new WaitForSeconds(config.FreeKnockbackTime);
            knockbackCount--;
        }

        public virtual void Kill(AnimationClip animClip = null)
        {
            if (characterState.Dying) { return; }

            if(animClip == null)
            {
                animClip = config.NormalDeathAnimations[UnityEngine.Random.Range(0, config.NormalDeathAnimations.Length)];
            }

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
            if (healthBar)
            {
                healthBar.SetFillAmount(GetHealthAsPercent());
            }
        }

        #endregion

        protected abstract void updateVolatilityUI();

    }
}
