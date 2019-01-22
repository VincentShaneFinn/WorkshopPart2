using UnityEngine;

namespace Finisher.Characters.Systems.Strategies {
    public abstract class DamageSystem : ScriptableObject
    {

        [SerializeField] protected float baseDamage = 10f;
        [SerializeField] private bool dealsKnockback = true;
        [SerializeField] private float knockbackRange = 0.5f;
        [SerializeField] private float knockbackDuration = 0.05f;
        //[SerializeField] private ParticleSystem particleSystem = null;

        public virtual void HitCharacter(GameObject damageSource, HealthSystem targetHealthSytem)
        {
            DealDamage(targetHealthSytem);
            DealKnockback(damageSource, targetHealthSytem);
        }

        protected void DealDamage(HealthSystem targetHealthSystem)
        {
            targetHealthSystem.DamageHealth(baseDamage);
        }

        protected void DealKnockback(GameObject damageSource, HealthSystem targetHealthSystem)
        {
            if (dealsKnockback)
            {
                //targetHealthSystem.Knockback();
                targetHealthSystem.Knockback(knockbackRange * damageSource.transform.forward, knockbackDuration);
            }
        }

        //Implement a play particleSystem
    }
}