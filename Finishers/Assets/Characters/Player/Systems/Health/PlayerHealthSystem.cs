using Finisher.Characters.Systems;
using Finisher.UI.Meters;

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
            healthBar = FindObjectOfType<UI.PlayerUIObjects>().gameObject.GetComponentInChildren<UI_HealthMeter>();
            healthBar.SetMeter(FindObjectOfType<UI.PlayerUIObjects>().HealthBar);
        }

        protected override void updateVolatilityUI()
        {
            return;
        }

    }
}
