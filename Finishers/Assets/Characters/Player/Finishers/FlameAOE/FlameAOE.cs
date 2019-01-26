using System.Collections.Generic;
using System.Collections;

using UnityEngine;

using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Player.Finishers {
    public class FlameAOE : MonoBehaviour
    {
        [SerializeField] private FlameAOEDamageSystem flameAOEDamageSystem;
        public float FinisherMeterCost { get { return flameAOEDamageSystem.FinisherMeterCost; } }

        [SerializeField] private float destroyInNSeconds = 1f;
        [SerializeField] private AnimationClip animationToPlay; public AnimationClip AnimationToPlay { get { return animationToPlay; } }
        // todo make configs for Finisher

        private CapsuleCollider capsuleCollider;

        protected HashSet<HealthSystem> hit = new HashSet<HealthSystem>();


        void Start()
        {
            Destroy(gameObject, destroyInNSeconds);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Player") { return; }

            var targetHealthSystem = col.gameObject.GetComponent<HealthSystem>();
            
            if (!hit.Add(targetHealthSystem))
            {
                return;
            }

            if (targetHealthSystem) // hit an enemy
            {
                flameAOEDamageSystem.HitCharacter(gameObject, targetHealthSystem);
            }
        }

    }
}