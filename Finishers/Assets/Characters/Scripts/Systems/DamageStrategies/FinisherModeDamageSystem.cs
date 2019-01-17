using UnityEngine;

namespace Finisher.Characters.Systems.Strategies
{
    public abstract class FinisherModeDamageSystem : DamageSystem
    {
        [SerializeField] private float volatilityDamage = 10f;

        protected void DealVolatilityDamage(HealthSystem targetHealthSystem)
        {
            targetHealthSystem.DamageVolatility(volatilityDamage);
        }
    }
}