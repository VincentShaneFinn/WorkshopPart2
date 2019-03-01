using UnityEngine;
using UnityEngine.UI;

using Finisher.UI;
using Finisher.UI.Meters;
using Finisher.Characters.Systems;

namespace Finisher.Characters.Enemies.Systems
{
    public class EnemyHealthSystem : HealthSystem
    {
        [SerializeField] private EnemyUI enemyCanvas;

        private UI_VolatilityMeter volatilityMeter;
        private UI_FinishableThresholdLine finishabilityLine;
        private Image volatilityMask;

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
                healthBar = GetComponentInChildren<UI_HealthMeter>();
                volatilityMeter = GetComponentInChildren<UI_VolatilityMeter>();
                volatilityMask = enemyCanvas.VolatilityMeterMask;
                finishabilityLine = GetComponentInChildren<UI_FinishableThresholdLine>();
                if (finishabilityLine)
                {
                    finishabilityLine.gameObject.SetActive(false);
                }
            }
        }

        private void setupVolatilityMeterToggle()
        {
            if (volatilityMask)
            {
                volatilityMask.gameObject.SetActive(false);
            }

            GameObject player = GameObject.FindGameObjectWithTag(TagNames.PlayerTag);
            if (player)
            {
                FinisherSystem playerFinisherSystem = player.GetComponent<FinisherSystem>();
                if (playerFinisherSystem)
                {
                    playerFinisherSystem.OnFinisherModeToggled += toggleVolatiltyMeter;
                }
            }
        }

        void OnDestroy()
        {
            GameObject player = GameObject.FindGameObjectWithTag(TagNames.PlayerTag);
            if (player)
            {
                FinisherSystem playerFinisherSystem = player.GetComponent<FinisherSystem>();
                if (playerFinisherSystem)
                {
                    playerFinisherSystem.OnFinisherModeToggled -= toggleVolatiltyMeter;
                }
            }
        }

        #region Override Kill(animclip)

        public override void Kill(AnimationClip animClip, bool overrideKillAnim = false)
        {
            base.Kill(animClip, overrideKillAnim);
            toggleEnemyCanvas(false);
        }

        #endregion

        #region Enemy UI

        protected override void updateFinishabilityUI()
        {
            //if (volatilityMeter)
            //{
            //    volatilityMeter.SetFillAmount(getVolaitilityAsPercent());
            //}
            if (!inFinisherMode)
            {
                healthBar.SetColor(false);
            }
            else
            {
                healthBar.SetColor(GetIsFinishable());
            }
        }

        private bool inFinisherMode = false;

        private void toggleVolatiltyMeter(bool enabled)
        {
            //currentVolatility = 0;

            //if (volatilityMask)
            //{
            //    volatilityMask.gameObject.SetActive(enabled);
            //}
            //if (volatilityMeter)
            //{
            //    volatilityMeter.SetFillAmountInstant(currentVolatility);
            //}
            inFinisherMode = enabled;
            if (finishabilityLine)
            {
                finishabilityLine.gameObject.SetActive(enabled);
            }
            updateHealthUI();
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