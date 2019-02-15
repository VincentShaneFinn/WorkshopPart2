using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Enemies;
using System;

namespace Finisher.Characters.Systems
{
    public class KnightCombatSystem : CombatSystem
    {

        AICharacterController character;
        EnemyAI enemyAI;
        Transform target;

        protected override void Start()
        {
            base.Start();

            character = GetComponent<AICharacterController>();
            enemyAI = GetComponent<EnemyAI>();
            var behaviors = animator.GetBehaviours<RushSequenceSMB>();
            foreach (var behavior in behaviors)
            {
                behavior.KnightCombatSystem = this;
            }
        }

        public void RushAttack(Transform target)
        {
            this.target = target;
            animator.SetTrigger("SpecialAttack");
            enemyAI.ForcedSequenceRunning = true;
        }

        public IEnumerator RushingCoroutine()
        {
            var startingPoint = transform.position;
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            character.LookAtTarget(target);

            while (!enemyAI.isPlayerInAttackRange())
            {
                character.LookAtTarget(target);
                yield return null;
            }

            animator.SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
            ResetRushing();
        }

        public void ResetRushing()
        {
            //do something when you have left the rushing state
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
            enemyAI.ForcedSequenceRunning = false;
        }

    }
}