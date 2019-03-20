using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Player.Finishers
{
    public class Blades : FinisherExecution
    {
        [SerializeField] public Blade blade;
        [SerializeField] private FinisherSkillsDamageSystem damageSystem;
        public override float FinisherMeterCost { get { return damageSystem.FinisherMeterCost; } }
        [SerializeField] private AnimationClip animationToPlay;
        public override AnimationClip AnimationToPlay { get { return animationToPlay; } }

        [SerializeField] private float destroyInNSeconds = .5f;
        // todo make configs for Finisher
        

        protected List<HealthSystem> hit = new List<HealthSystem>();


        void Start()
        {
            Destroy(gameObject, destroyInNSeconds);
            combatSystem = GameObject.FindGameObjectWithTag(TagNames.PlayerTag).GetComponent<CombatSystem>();
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Player") { return; }

            var targetHealthSystem = col.gameObject.GetComponent<HealthSystem>();

            if (targetHealthSystem) // hit an enemy
            {
                hit.Add(targetHealthSystem);
                combatSystem.CallCameraShakeEvent(0.5f, combatSystem.HeavyAttackDamageSystem.KnockbackDuration * 1.5f);
            }

            
        }
        private void OnDestroy()
        {
            foreach (HealthSystem h in hit) {
                Blade b= Instantiate(blade,transform.position + Vector3.up * 1.5f,transform.rotation); 
                b.target = h;
                b.damageSystem = damageSystem;
                b.Launch();
            }
        }
    }
}