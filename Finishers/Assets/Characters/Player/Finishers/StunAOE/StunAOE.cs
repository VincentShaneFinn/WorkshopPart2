﻿using System.Collections.Generic;
using System.Collections;

using UnityEngine;

using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Player.Finishers {
    public class StunAOE : FinisherExecution
    {
        [SerializeField] private FinisherSkillsDamageSystem stunAOEDamageSystem;
        public override float FinisherMeterCost { get { return stunAOEDamageSystem.FinisherMeterCost; } }
        [SerializeField] private AnimationClip animationToPlay; public override AnimationClip AnimationToPlay { get { return animationToPlay; } }

        [SerializeField] private float destroyInNSeconds = 1f;
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
                stunAOEDamageSystem.HitCharacter(gameObject, targetHealthSystem);
                //TODO: add this to the damage system
                targetHealthSystem.GetComponent<CharacterState>().Stun(3);
            }
        }

    }
}