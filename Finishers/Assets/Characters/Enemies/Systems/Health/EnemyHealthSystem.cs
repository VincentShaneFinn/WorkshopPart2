using UnityEngine;
using UnityEngine.UI;

using Finisher.UI;
using Finisher.Characters.Systems;

namespace Finisher.Characters.Enemies.Systems
{
    public class EnemyHealthSystem : HealthSystem
    {
        [SerializeField] private EnemyUI enemyCanvas;

        private Image volatilityMeter;

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
                healthBar = enemyCanvas.HealthBar;
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
                volatilityMeter.fillAmount = GetVolaitilityAsPercent();
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