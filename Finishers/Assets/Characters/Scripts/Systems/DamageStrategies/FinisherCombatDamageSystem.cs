using UnityEngine;

namespace Finisher.Characters.Systems.Strategies
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/Damage/FinisherCombat"))]
    public class FinisherCombatDamageSystem : FinisherModeDamageSystem
    {
        public override void HitCharacter(HealthSystem targetHealthSystem)
        {
            DealDamage(targetHealthSystem);
            DealKnockback(targetHealthSystem);
            DealVolatilityDamage(targetHealthSystem);
        }
    }
}