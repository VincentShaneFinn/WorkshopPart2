using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Finisher.UI;

namespace Finisher.Characters
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterAnimator))]
    // todo polymorphise To Player and Enemy Health System
    public class HealthSystem : MonoBehaviour
    {
        [Tooltip("Player's is set automatically")]
        [SerializeField] private EnemyUI enemyCanvas;

        [SerializeField] HealthSystemConfig config;
        [SerializeField] float maxHealth = 100f;
        [SerializeField] float maxVolatility = 100f;
        [SerializeField] int KnockbackLimit = 2;
        [SerializeField] float FreeKnockbackTime = 1f;

        public delegate void KnockedBack();
        public event KnockedBack OnKnockBack;
        // Note: we do not guarantee this has a subscriber, so check if null when calling

        private float currentHealth;
        private float currentVolatility;
        private int knockbackCount;

        [HideInInspector] public CharacterAnimator character;
        [HideInInspector] public Animator Animator;
        private Slider healthSlider;
        private Slider volatilityMeter;

        void Start()
        {
            character = GetComponent<CharacterAnimator>();

            Animator = GetComponent<Animator>();


            setEnemySliders();
            setPlayerHealthSlider();
            increaseHealth(maxHealth);
            decreaseVolatility(maxVolatility);
            setupVolatilityMeter();
        }

        private void setEnemySliders()
        {
            if (enemyCanvas)
            {
                healthSlider = enemyCanvas.HealthSlider;
                volatilityMeter = enemyCanvas.VolatilityMeter;
            }
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
            if (Input.GetKeyDown(KeyCode.Alpha8) && character.Strafing)
            {
                Animator.SetTrigger(AnimConstants.Parameters.INVULNERABLEACTION_TRIGGER);
            }
        }

        #region Public Interface

        #region Change Health

        public void DamageHealth(float damage)
        {
            //Dont deal damage if dodging
            if (character.Invulnerable) { return; }

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

        // todo knockback is currently really a stagger, and we need to add a knockback with a movement vector
        #region Knockback And Kill

        public void Knockback()
        {
            Knockback(config.KnockbackAnimations[UnityEngine.Random.Range(0, config.KnockbackAnimations.Length)]);
        }
        public void Knockback(AnimationClip animClip)
        {
            if (character.Dying) { return; }
            character.SetTriggerOverride(AnimConstants.Parameters.KNOCKBACK_TRIGGER, AnimConstants.OverrideIndexes.KNOCKBACK_INDEX, animClip);
            knockbackCount++;
            StartCoroutine(releaseCountAfterDelay());
            if (OnKnockBack != null)
            {
                OnKnockBack();
            }
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
        public void Kill(AnimationClip animClip)
        {
            if (character.Dying) { return; }
            currentHealth = 0;
            updateHealthUI();
            toggleEnemyCanvas(false);
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

        private void toggleEnemyCanvas(bool enabled)
        {
            if (enemyCanvas)
            {
                enemyCanvas.gameObject.SetActive(enabled);
            }
        }

        #endregion
    }
}
