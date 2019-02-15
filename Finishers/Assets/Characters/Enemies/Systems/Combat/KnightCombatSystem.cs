using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Enemies;

namespace Finisher.Characters.Systems
{
    public class KnightCombatSystem : CombatSystem
    {

        AICharacterController character;
        EnemyAI enemyAI;

        protected override void Start()
        {
            base.Start();

            character = GetComponent<AICharacterController>();
            enemyAI = GetComponent<EnemyAI>();
        }

        public void RushAttack(Transform target)
        {
            StartCoroutine(RushAttackSequence(target));
        }

        IEnumerator RushAttackSequence(Transform target)
        {

            //Setup ---------------------------------

            Animator animator = GetComponent<Animator>();
            animator.SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
            animator.SetInteger("SpecialAttackIndex", 1);

            yield return new WaitUntil(() => animator.IsInTransition(0));

            animator.SetInteger("SpecialAttackIndex", 0); //Reset Attack back to normal

            yield return new WaitUntil(() => !animator.IsInTransition(0));

            enemyAI.ForcedSequenceRunning = true;

            while (animator.GetCurrentAnimatorStateInfo(0).IsName("RushSetup") && !animator.IsInTransition(0)) //We successfully started the rush
            {
                yield return null;
            }

            if (CurrentAttackType != AttackType.Special)
            {
                resetRushing();
                yield break;
            }



            //Rushing -----------------------------------
            var startingPoint = transform.position;

            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

            yield return new WaitUntil(() => !GetComponent<Animator>().IsInTransition(0));

            while (animator.GetCurrentAnimatorStateInfo(0).IsName("Rushing") &&
                !animator.IsInTransition(0) &&
                !enemyAI.isPlayerInAttackRange())
            {
                character.LookAtTarget(target);
                yield return null;
            }

            if (CurrentAttackType != AttackType.Special)
            {
                resetRushing();
                yield break;
            }

            //Final Attack ---------------------------------

            animator.SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);

            yield return new WaitUntil(() => !animator.IsInTransition(0));

            while (animator.GetCurrentAnimatorStateInfo(0).IsName("RushAttack") && animator.IsInTransition(0))
            {
                yield return null;
            }

            //Do something at the end
            resetRushing();
        }

        private void resetRushing()
        {
            //do something when you have left the rushing state
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
            enemyAI.ForcedSequenceRunning = false;
        }

    }
}