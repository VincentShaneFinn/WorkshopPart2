using UnityEngine;

namespace Finisher.Characters.Systems.Strategies
{
    public abstract class FinisherModeDamageSystem : DamageSystem
    {
        [SerializeField] private float volatilityDamage = 10f;

        public override void HitCharacter(HealthSystem targetHealthSytem)
        {
            base.HitCharacter(targetHealthSytem);
            DealVolatilityDamage(targetHealthSytem);
        }

        protected void DealVolatilityDamage(HealthSystem targetHealthSystem)
        {
            targetHealthSystem.DamageVolatility(volatilityDamage);
        }
    }
}