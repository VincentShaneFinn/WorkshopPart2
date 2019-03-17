using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;
using Finisher.UI.Meters;
using UnityEngine;

namespace Finisher.Characters.Player.Systems
{
    public class PlayerHealthSystem : HealthSystem
    {

        protected override void Start()
        {

            setPlayerHealthSlider();

            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            if (FinisherInput.HealCheat() && !characterState.Dying)
            {
                IncreaseHealth(config.MaxHealth);
            }
            if (FinisherInput.InvulnerabilityCheat())
            {
                invulnerableCheat = !invulnerableCheat;
            }
        }

        private bool invulnerableCheat = false;

        public override void DamageHealth(float damage, DamageSystem damageSource)
        {
            if (invulnerableCheat) {
                if (GetHealthAsPercent() > .20)
                {
                    base.DamageHealth(damage, damageSource);
                }
            }
            else
            {
                base.DamageHealth(damage, damageSource);
            }
        }

        private void setPlayerHealthSlider()
        {
            healthBar = FindObjectOfType<UI.PlayerUIObjects>().gameObject.GetComponentInChildren<UI_HealthMeter>();
        }

        protected override void updateFinishabilityUI()
        {
            return;
        }

        public override void CutInHalf()
        {
            Instantiate(config.TopHalf, transform.position, transform.rotation);
            Instantiate(config.BottomHalf, transform.position, transform.rotation);
            Kill();
            Destroy(gameObject);
            gameObject.SetActive(false);
        }

    }
}
