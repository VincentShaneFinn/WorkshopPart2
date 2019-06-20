using Finisher.Characters.Systems.Strategies;
using Finisher.UI.Meters;
using System.Collections;
using UnityEngine;

namespace Finisher.Characters.Systems
{
    [RequireComponent(typeof(CharacterAnimator))]
    public abstract class HealthSystem : MonoBehaviour
    {
        [SerializeField] protected HealthConfig config;

        protected int knockbackCount;
        protected bool immuneToKnockback = false;
        protected CharacterState characterState;
        protected UI_HealthMeter healthBar;
        private AnimOverrideSetter animOverrideHandler;
        protected float currentHealth { get; set; }
        protected float currentVolatility { get; set; }

        #region Delegates

        public delegate void KnockedBack();

        public delegate void TookDamage();

        public event KnockedBack OnKnockBack;

        public event TookDamage OnDamageTaken;

        protected void CallDamageTakenEvent()
        {
            if (OnDamageTaken != null)
            {
                OnDamageTaken();
            }
        }

        private void CallKnockbackEvent()
        {
            if (OnKnockBack != null)
            {
                OnKnockBack();
            }
        }

        #endregion Delegates

        protected virtual void Start()
        {
            characterState = GetComponent<CharacterState>();
            animOverrideHandler = GetComponent<AnimOverrideSetter>();

            IncreaseHealth(config.MaxHealth);
            decreaseVolatility(config.MaxVolatility);
        }

        protected virtual void Update()
        {
            if (!characterState.Dying)
            {
                IncreaseHealth(config.RegenPerSecond * Time.deltaTime);
            }
        }

        #region Public Interface

        #region Change Health

        public virtual void DamageHealth(float damage, DamageSystem damageSource)
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
            if (currentHealth > config.MaxHealth - Mathf.Epsilon)
            {
                currentHealth = config.MaxHealth;
            }
            updateHealthUI();
        }

        public float GetHealthAsPercent()
        {
            return currentHealth / config.MaxHealth;
        }

        protected void decreaseHealth(float damage)
        {
            currentHealth -= damage;
            if (currentHealth < Mathf.Epsilon)
            {
                currentHealth = 0;
            }
            updateHealthUI();
        }

        #endregion Change Health

        #region Change Volatility

        public void DamageVolatility(float amount)
        {
            increaseVolatility(amount);
            checkVolatilityFull();
        }

        public bool GetIsFinishable()
        {
            return GetHealthAsPercent() < (config.FinishableLowerBound + (config.FinishableUpperBound - config.FinishableLowerBound) * getVolaitilityAsPercent());
        }

        protected float getVolaitilityAsPercent()
        {
            return currentVolatility / config.MaxVolatility;
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
            updateFinishabilityUI();
        }

        private void decreaseVolatility(float amount)
        {
            currentVolatility -= amount;
            if (currentVolatility <= Mathf.Epsilon)
            {
                currentVolatility = 0;
            }
            updateFinishabilityUI();
        }

        private void checkVolatilityFull()
        {
            if (currentVolatility >= config.MaxVolatility)
            {
                //character.Staggered = true; // todo if stagger implemented here, protect from leaving grab mode
            }
        }

        #endregion Change Volatility

        #region Knockback And Kill

        public void Knockback(Vector3 knockbackVector, float knockbackTime = 0.1f, AnimationClip animClip = null, bool force = false)
        {
            //TODO: Add a method to override the knockback limiter
            if (characterState.Dying || knockbackCount >= config.KnockbackLimit || (immuneToKnockback && !force) || characterState.FinisherModeActive) { return; }

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
            Knockback((Vector3.Normalize(transform.position - damageSource.transform.position) * knockbackRange), knockbackTime, animClip, force: true);
        }

        public void Knockback(AnimationClip animClip = null, bool force = false)
        {
            //TODO: Add a method to override the knockback limiter
            if (characterState.Dying || knockbackCount >= config.KnockbackLimit || (immuneToKnockback && !force) || characterState.FinisherModeActive) { return; }

            if (animClip == null)
            {
                animClip = config.KnockbackAnimations[UnityEngine.Random.Range(0, config.KnockbackAnimations.Length)];
            }

            enterKnockbackState(animClip);
            CallKnockbackEvent();

            knockbackCount++;
            StartCoroutine(releaseCountAfterDelay());
        }

        public virtual void Kill(AnimationClip animClip = null, bool overrideKillAnim = false)
        {
            if (characterState.Dying && !overrideKillAnim) { return; }

            if (animClip == null)
            {
                //animClip = config.NormalDeathAnimations[UnityEngine.Random.Range(0, config.NormalDeathAnimations.Length)];
                CutInHalf();
            }

            currentHealth = 0;
            updateHealthUI();
            enterDyingState(animClip);
        }

        public virtual void Revive()
        {
            AnimationClip animClip = animClip = config.NormalDeathAnimations[UnityEngine.Random.Range(0, config.NormalDeathAnimations.Length)];
            currentHealth = 100;
            updateHealthUI();
            ReviveAnimate(animClip);
        }

        public virtual void CutInHalf()
        {
            Instantiate(config.TopHalf, transform.position, transform.rotation);
            Instantiate(config.BottomHalf, transform.position, transform.rotation);
            Destroy(gameObject);
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

        private void enterDyingState(AnimationClip animClip)
        {
            characterState.DyingState.Kill();
            animOverrideHandler.SetBoolOverride(AnimConstants.Parameters.DYING_BOOL, true, AnimConstants.OverrideIndexes.DEATH_INDEX, animClip);
        }

        private void ReviveAnimate(AnimationClip animClip)
        {
            characterState.DyingState.Revive();
            animOverrideHandler.SetBoolOverride(AnimConstants.Parameters.DYING_BOOL, false, AnimConstants.OverrideIndexes.DEATH_INDEX, animClip);
        }

        #endregion Knockback And Kill

        public bool WillDamageKill(float damage)
        {
            if (currentHealth - damage <= Mathf.Epsilon)
            {
                return true;
            }
            return false;
        }

        #endregion Public Interface

        #region updateUI

        protected void updateHealthUI()
        {
            if (healthBar)
            {
                healthBar.SetFillAmount(GetHealthAsPercent());
                updateFinishabilityUI();
            }
        }

        #endregion updateUI

        protected abstract void updateFinishabilityUI();
    }
}