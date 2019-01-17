using UnityEngine;

namespace Finisher.Characters.Systems.Strategies
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/Damage/FinisherSkills/Execution"))]
    public class ExecutionDamageSystem : FinisherSkillsDamageSystem
    {
        [SerializeField] private bool IsOneShot = true;

        public override void HitCharacter(HealthSystem targetHealthSytem)
        {
            if (IsOneShot)
            {
                targetHealthSytem.Kill();
            }
            else
            {
                base.HitCharacter(targetHealthSytem);
            }
        }

        //TODO: Special things that happen when the enemy is executed
    }
}