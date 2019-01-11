using UnityEngine;
using UnityEngine.UI;

using Finisher.UI;

namespace Finisher.Characters.Systems
{
    public class EnemyHealthSystem : HealthSystem
    {
        [SerializeField] private EnemyUI enemyCanvas;
        [SerializeField] float maxVolatility = 100f;

        protected float currentVolatility;

        private Slider volatilityMeter;


        protected override void Start()
        {
            setEnemySliders();
            setupVolatilityMeterToggle();
            decreaseVolatility(maxVolatility);

            base.Start();
        }

        private void setEnemySliders()
        {
            if (enemyCanvas)
            {
                healthSlider = enemyCanvas.HealthSlider;
                volatilityMeter = enemyCanvas.VolatilityMeter;
            }
        }

        private void setupVolatilityMeterToggle()
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

        #region Change Volatility

        public override void DamageVolatility(float amount)
        {
            increaseVolatility(amount);
            checkVolatilityFull();
        }

        private void increaseVolatility(float amount)
        {
            currentVolatility += amount;
            if (currentVolatility > maxVolatility - Mathf.Epsilon)
            {
                currentVolatility = maxVolatility;
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

        #region Override Kill(animclip)

        public override void Kill(AnimationClip animClip)
        {
            base.Kill(animClip);
            toggleEnemyCanvas(false);
        }

        #endregion

        #region Enemy UI

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