using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Finisher.Characters.Systems
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterAnimator))]
    public abstract class HealthSystem : MonoBehaviour
    {
        [SerializeField] private HealthConfig config;
        [SerializeField] float maxHealth = 100f;
        [SerializeField] int KnockbackLimit = 2;
        [SerializeField] float FreeKnockbackTime = 1f;

        protected float currentHealth;
        protected int knockbackCount;

        private CharacterState characterState;
        private AnimOverrideHandler animOverrideHandler;
        protected Slider healthSlider;

        protected virtual void Start()
        {
            characterState = GetComponent<CharacterState>();
            animOverrideHandler = GetComponent<AnimOverrideHandler>();

            increaseHealth(maxHealth);
        }

        #region Public Interface

        #region Change Health

        public void DamageHealth(float damage)
        {
            //Dont deal damage if dodging
            if (characterState.Invulnerable) { return; }

            decreaseHealth(damage);

            if (knockbackCount < KnockbackLimit)
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

        public virtual void DamageVolatility(float amount)
        {
            
        }

        // todo knockback is currently really a stagger, and we need to add a knockback with a movement vector
        #region Knockback And Kill

        public void Knockback()
        {
            Knockback(config.KnockbackAnimations[UnityEngine.Random.Range(0, config.KnockbackAnimations.Length)]);
        }
        public virtual void Knockback(AnimationClip animClip)
        {
            if (characterState.Dying) { return; }
            enterKnockbackState(animClip);
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
            yield return new WaitForSeconds(FreeKnockbackTime);
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

    }
}
