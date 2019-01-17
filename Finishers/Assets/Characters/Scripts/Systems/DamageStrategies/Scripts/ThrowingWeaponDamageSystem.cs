using UnityEngine;

namespace Finisher.Characters.Systems.Strategies
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/Damage/FinisherSkills/ThrowingWeapon"))]
    public class ThrowingWeaponDamageSystem : FinisherSkillsDamageSystem
    {
        [SerializeField] private AnimationClip StandingDeathAnimClip;

        public override void HitCharacter(HealthSystem targetHealthSytem)
        {
            if (targetHealthSytem.WillDamageKill(baseDamage))
            {
                targetHealthSytem.Kill(StandingDeathAnimClip);
            }
            else
            {
                base.HitCharacter(targetHealthSytem);
            }
        }

    }
}