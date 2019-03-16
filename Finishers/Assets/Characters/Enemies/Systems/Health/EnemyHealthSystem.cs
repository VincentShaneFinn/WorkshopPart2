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
        [SerializeField] private ParticleSystem enemyWeapon;

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

        protected override void Update()
        {
            base.Update();
            if(characterState.HeavyAttacking || characterState.getAnimator().GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.RUSHING_ATTACK_STATE))
            {
                immuneToKnockback = true;
                enemyWeapon.gameObject.SetActive(true);
                //enemyWeapon.startColor = uninteruptableAttackColor;
            }
            else {
                immuneToKnockback = false;
                enemyWeapon.gameObject.SetActive(false);
            }
        }

        #region Override Kill(animclip)

        public override void Kill(AnimationClip animClip, bool overrideKillAnim = false)
        {
            base.Kill(animClip, overrideKillAnim);
            toggleEnemyCanvas(false);
        }

        public override void Revive()
        {
            base.Revive();
            toggleEnemyCanvas(true);
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
            if (finishabilityLine)
            {
                finishabilityLine.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, (config.FinishableLowerBound + (config.FinishableUpperBound - config.FinishableLowerBound) * getVolaitilityAsPercent()) * 100, 0);
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

        public void toggleEnemyCanvas(bool enabled)
        {
            if (enemyCanvas)
            {
                enemyCanvas.gameObject.SetActive(enabled);
            }
        }

        #endregion
    }
}