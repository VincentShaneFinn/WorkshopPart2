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

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Alpha3) && !characterState.Dying)
            {
                IncreaseHealth(config.MaxHealth);
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
