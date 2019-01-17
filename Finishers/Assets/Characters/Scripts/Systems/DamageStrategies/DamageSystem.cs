using UnityEngine;

namespace Finisher.Characters.Systems.Strategies {
    public abstract class DamageSystem : ScriptableObject
    {

        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private bool dealsKnockback = true;
        //[SerializeField] private float knockbackRange = 0;
        //[SerializeField] private ParticleSystem particleSystem = null;

        public virtual void HitCharacter(HealthSystem targetHealthSytem)
        {
            DealDamage(targetHealthSytem);
            DealKnockback(targetHealthSytem);
        }

        protected void DealDamage(HealthSystem targetHealthSystem)
        {
            targetHealthSystem.DamageHealth(baseDamage);
        }

        protected void DealKnockback(HealthSystem targetHealthSystem)
        {
            //TODO: allow health systems knockback to take a movementVector
            if (dealsKnockback)
            {
                targetHealthSystem.Knockback();
            }
        }

        //Implement a play particleSystem
    }
}