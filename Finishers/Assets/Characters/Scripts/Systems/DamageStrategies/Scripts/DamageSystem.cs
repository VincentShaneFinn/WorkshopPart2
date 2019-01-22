using System;
using UnityEngine;

namespace Finisher.Characters.Systems.Strategies {
    public abstract class DamageSystem : ScriptableObject
    {

        [SerializeField] protected float baseDamage = 10f;
        [SerializeField] private bool dealsKnockback = true;
        [SerializeField] private float knockbackRange = 0.5f;
        [SerializeField] private float knockbackDuration = 0.05f;
        //[SerializeField] private ParticleSystem particleSystem = null;
        [SerializeField] private ParticleEventSystem particleEventSystem = null;

        public virtual void HitCharacter(GameObject damageSource, HealthSystem targetHealthSytem)
        {
            DealDamage(targetHealthSytem);
            DealKnockback(damageSource, targetHealthSytem);
            playParticle(targetHealthSytem);
        }

        private void playParticle(HealthSystem targetHealthSystem)
        {
            if (particleEventSystem != null)
            {
                
                particleEventSystem.play(targetHealthSystem.transform.TransformPoint(new Vector3(0, 1.2f, 0)), targetHealthSystem.transform.rotation);
            }
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