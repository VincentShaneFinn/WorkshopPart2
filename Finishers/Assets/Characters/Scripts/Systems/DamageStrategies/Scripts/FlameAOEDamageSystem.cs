using UnityEngine;

namespace Finisher.Characters.Systems.Strategies
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/Damage/FinisherSkills/FlameAOE"))]
    public class FlameAOEDamageSystem : FinisherSkillsDamageSystem
    {
        override protected void DealKnockback(GameObject damageSource, HealthSystem targetHealthSystem)
        {
            if (dealsKnockback)
            {
                targetHealthSystem.KnockbackOutwards(damageSource, knockbackRange, knockbackDuration);
            }
        }
    }
}