using UnityEngine;

namespace Finisher.Characters.Systems
{
    public class PlayerHealthSystem : HealthSystem
    {

        protected override void Start()
        {

            setPlayerHealthSlider();

            base.Start();
        }

        private void setPlayerHealthSlider()
        {
            healthSlider = FindObjectOfType<UI.PlayerUIObjects>().HealthSlider;
        }

        protected override void updateVolatilityUI()
        {
            return;
        }

    }
}
