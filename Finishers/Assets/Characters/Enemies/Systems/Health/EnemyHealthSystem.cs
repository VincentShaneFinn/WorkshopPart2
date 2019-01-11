using UnityEngine;
using UnityEngine.UI;

using Finisher.UI;

namespace Finisher.Characters.Systems
{
    public class EnemyHealthSystem : HealthSystem
    {
        [SerializeField] private EnemyUI enemyCanvas;

        private Slider volatilityMeter;


        protected override void Start()
        {
            setEnemySliders();
            setupVolatilityMeterToggle();

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

        #region Override Kill(animclip)

        public override void Kill(AnimationClip animClip)
        {
            base.Kill(animClip);
            toggleEnemyCanvas(false);
        }

        #endregion

        #region Enemy UI

        protected override void updateVolatilityUI()
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