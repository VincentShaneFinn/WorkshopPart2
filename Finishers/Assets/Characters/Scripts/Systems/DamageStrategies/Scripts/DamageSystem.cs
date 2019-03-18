using System;
using UnityEngine;

namespace Finisher.Characters.Systems.Strategies {
    [CreateAssetMenu(menuName = ("Finisher/Systems/Damage/SimpleDamage"))]
    public class DamageSystem : ScriptableObject
    {

        [SerializeField] protected float baseDamage = 10f;
        public float BaseDamage { get { return baseDamage; } }

        [SerializeField] protected bool dealsKnockback = true;
        [SerializeField] protected float knockbackRange = 0.5f;

        [SerializeField] protected float knockbackDuration = 0.05f;
        public float KnockbackDuration { get { return knockbackDuration; } }

        [SerializeField] private ParticleEventSystem particleEventSystem = null;


        public virtual void HitCharacter(GameObject damageSource, HealthSystem targetHealthSytem, float damageMultiplier=1, float bonusDamage=0)

        {

            DealDamage(targetHealthSytem,damageMultiplier,bonusDamage);
            DealKnockback(damageSource, targetHealthSytem);
            playParticle(targetHealthSytem);
        }

        protected virtual void DealDamage(HealthSystem targetHealthSystem, float damageMultiplier, float bonusDamage)
        {

            targetHealthSystem.DamageHealth(baseDamage*damageMultiplier + bonusDamage, this);

        }

        protected virtual void DealKnockback(GameObject damageSource, HealthSystem targetHealthSystem)
        {
            if (dealsKnockback)
            {
                //targetHealthSystem.Knockback();
                targetHealthSystem.Knockback(knockbackRange * damageSource.transform.forward, knockbackDuration);
            }
        }

        protected void playParticle(HealthSystem targetHealthSystem)
        {
            if (particleEventSystem != null)
            {
                particleEventSystem.play(targetHealthSystem.transform.TransformPoint(new Vector3(0, 1.2f, 0)), targetHealthSystem.transform.rotation);
            }
        }
    }
}