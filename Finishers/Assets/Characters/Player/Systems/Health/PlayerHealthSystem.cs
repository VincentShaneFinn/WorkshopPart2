using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Player.Systems
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
            healthBar = FindObjectOfType<UI.PlayerUIObjects>().gameObject.GetComponent<HealthBar>();
            healthBar.SetHealthBar(FindObjectOfType<UI.PlayerUIObjects>().HealthBar);
        }

        protected override void updateVolatilityUI()
        {
            return;
        }

    }
}
