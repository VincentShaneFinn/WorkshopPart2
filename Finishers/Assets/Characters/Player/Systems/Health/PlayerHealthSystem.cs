using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;
using Finisher.UI.Meters;
using UnityEngine.SceneManagement;

namespace Finisher.Characters.Player.Systems
{
    public class PlayerHealthSystem : HealthSystem
    {
        private bool invulnerableCheat = false;

        public override void DamageHealth(float damage, DamageSystem damageSource)
        {
            if (invulnerableCheat)
            {
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

        public override void CutInHalf()
        {
            SceneManager.LoadScene(0);
        }

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

        protected override void updateFinishabilityUI()
        {
            return;
        }

        private void setPlayerHealthSlider()
        {
            healthBar = FindObjectOfType<UI.PlayerUIObjects>().gameObject.GetComponentInChildren<UI_HealthMeter>();
        }
    }
}