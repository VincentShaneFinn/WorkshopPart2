﻿using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Player.Finishers {
    public class FlameAOE : FinisherExecution
    {
        [SerializeField] private FinisherSkillsDamageSystem flameAOEDamageSystem;
        public override float FinisherMeterCost { get { return flameAOEDamageSystem.FinisherMeterCost; } }
        [SerializeField] private AnimationClip animationToPlay; public override AnimationClip AnimationToPlay { get { return animationToPlay; } }

        [SerializeField] private float destroyInNSeconds = 1f;
        // todo make configs for Finisher

        private CapsuleCollider capsuleCollider;


        void Start()
        {
            Destroy(gameObject, destroyInNSeconds);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Player") { return; } 

            var targetHealthSystem = col.gameObject.GetComponent<HealthSystem>();
            if (targetHealthSystem) // hit an enemy
            {
                flameAOEDamageSystem.HitCharacter(gameObject, targetHealthSystem);
            }
        }

    }
}