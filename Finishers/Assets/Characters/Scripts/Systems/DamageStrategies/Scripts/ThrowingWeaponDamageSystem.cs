using UnityEngine;

namespace Finisher.Characters.Systems.Strategies
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/Damage/FinisherSkills/ThrowingWeapon"))]
    public class ThrowingWeaponDamageSystem : FinisherSkillsDamageSystem
    {
        [SerializeField] private AnimationClip StandingDeathAnimClip;

        public override void HitCharacter(GameObject damageSource, HealthSystem targetHealthSytem, float damageMultiplier = 1, float bonusDamage = 0)
        {
            if (targetHealthSytem.WillDamageKill(baseDamage))
            {
                targetHealthSytem.Kill(StandingDeathAnimClip);
            }
            else
            {
                base.HitCharacter(damageSource, targetHealthSytem);
            }
        }

    }
}