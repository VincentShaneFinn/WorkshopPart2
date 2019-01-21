using System;
using UnityEngine;

namespace Finisher.Characters.Systems.Strategies {
    public abstract class DamageSystem : ScriptableObject
    {

        [SerializeField] protected float baseDamage = 10f;
        [SerializeField] private bool dealsKnockback = true;
        //[SerializeField] private float knockbackRange = 0;
        [SerializeField] private ParticleEventSystem particleEventSystem = null;

        public virtual void HitCharacter(HealthSystem targetHealthSytem)
        {
            DealDamage(targetHealthSytem);
            DealKnockback(targetHealthSytem);
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