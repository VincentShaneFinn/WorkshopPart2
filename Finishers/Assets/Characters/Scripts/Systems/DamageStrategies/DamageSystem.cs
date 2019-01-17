using UnityEngine;

namespace Finisher.Characters.Systems.Strategies {
    public abstract class DamageSystem : ScriptableObject
    {

        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private bool dealsKnockback = true;
        [SerializeField] private float knockbackRange = 0;
        [SerializeField] private ParticleSystem particleSystem = null;

        public virtual void Hit(HealthSystem target)
        {
            if (target)
            {
                DealDamage(target);
                DealKnockback(target);
            }
        }

        protected void DealDamage(HealthSystem target)
        {
            target.DamageHealth(baseDamage);
        }

        protected void DealKnockback(HealthSystem target)
        {
            //TODO: allow health systems knockback to take a movementVector
            if (dealsKnockback)
            {
                target.Knockback();
            }
        }

        //Implement a play particleSystem
    }
}