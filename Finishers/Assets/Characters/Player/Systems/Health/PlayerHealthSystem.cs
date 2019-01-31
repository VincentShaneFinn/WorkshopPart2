using Finisher.Characters.Systems;
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

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3) && !characterState.Dying)
            {
                IncreaseHealth(config.MaxHealth);
            }
        }

        public override void DamageHealth(float damage)
        {
            if (GetHealthAsPercent() > .20)
            {
                base.DamageHealth(damage);
            }
        }

        private void setPlayerHealthSlider()
        {
            healthBar = FindObjectOfType<UI.PlayerUIObjects>().gameObject.GetComponentInChildren<UI_HealthMeter>();
        }

        protected override void updateVolatilityUI()
        {
            return;
        }

    }
}
