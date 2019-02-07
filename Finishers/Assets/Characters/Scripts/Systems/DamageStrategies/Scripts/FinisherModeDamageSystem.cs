using UnityEngine;

namespace Finisher.Characters.Systems.Strategies
{
    public abstract class FinisherModeDamageSystem : DamageSystem
    {
        [SerializeField] private float volatilityDamage = 5f;

        public override void HitCharacter(GameObject damageSource, HealthSystem targetHealthSytem, float damageMultiplier = 1, float bonusDamage = 0)
        {
            base.HitCharacter(damageSource, targetHealthSytem,bonusDamage:bonusDamage);
            DealVolatilityDamage(targetHealthSytem);
        }

        protected void DealVolatilityDamage(HealthSystem targetHealthSystem)
        {
            float newVolatilityDamage = volatilityDamage + ((volatilityDamage / 2) * ((1 - targetHealthSystem.GetHealthAsPercent()) * 2.5f));
            targetHealthSystem.DamageVolatility(newVolatilityDamage);
        }
    }
}