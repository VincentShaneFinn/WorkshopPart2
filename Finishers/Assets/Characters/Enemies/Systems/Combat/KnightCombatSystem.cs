using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Enemies;
using System;

namespace Finisher.Characters.Systems
{
    public class KnightCombatSystem : CombatSystem
    {

        public bool IsPerformingSpecialAttack;

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
        }

        public IEnumerator RushingCoroutine()
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            character.LookAtTarget(target);

            while (!enemyAI.isPlayerInAttackRange())
            {
                character.LookAtTarget(target);
                yield return null;
            }

            animator.SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
        }

        public void ResetRushing()
        {
            //do something when you have left the rushing state
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        }

        public IEnumerator MonitorSpecialAttackStatus()
        {
            IsPerformingSpecialAttack = true;

            yield return new WaitForSeconds(.26f);

            yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.SPECIAL_ATTACK_SEQUENCE_TAG));

            ResetRushing();
            IsPerformingSpecialAttack = false;
            StopAllCoroutines();
        }
    }
}