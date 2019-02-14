using System.Collections;
using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Enemies
{
    public class KnightAI : EnemyAI
    {

        private float savedNormalAttackRadius;

        private bool useRushAttack = false;
        private const float RUSH_ATTACK_RADIUS = 6f;

        private bool tempinvokedSetup = false; 
        
        protected override void Start()
        {
            base.Start();

            savedNormalAttackRadius = attackRadius;

        }

        private void setContext()
        {
            attackRadius = RUSH_ATTACK_RADIUS;
            useRushAttack = true;

            tempinvokedSetup = false;
        }

        protected override void makeAttackDecision()
        {
            //if (!(currentState == EnemyState.Idle || currentState == EnemyState.ReturningHome))
            //{
            //    if (!tempinvokedSetup)
            //    {
            //        Invoke("setContext", Random.Range(3, 5));

            //        tempinvokedSetup = true;
            //    }
            //}
            //else
            //{
            //    attackRadius = savedNormalAttackRadius;
            //    useRushAttack = false;
            //}
        }

        protected override void attackPlayer()
        {
            if (useRushAttack)
            {
                useRushAttack = false;
                StartCoroutine(RushAttackSequence());
                attackRadius = savedNormalAttackRadius;
            }
            else
            {
                base.attackPlayer();
            }
        }

        //TODO: move this to an offshoot of the combatSytem
        //TODO: can we make a clean way to disable the nav mesh
        IEnumerator RushAttackSequence()
        {

            //Setup ---------------------------------

            Animator animator = GetComponent<Animator>();
            animator.SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
            animator.SetInteger("SpecialAttackIndex", 1);
            yield return null;

            animator.SetInteger("SpecialAttackIndex", 0); //Reset back to normal

            yield return new WaitUntil(() => !animator.IsInTransition(0));

            while (animator.GetCurrentAnimatorStateInfo(0).IsName("RushSetup") && !animator.IsInTransition(0))
            {
                yield return null;
            }

            if(combatSystem.CurrentAttackType != AttackType.Special)
            {
                resetRushing();
                yield break;
            }



            //Rushing -----------------------------------
            var startingPoint = transform.position;
            character.LookAtTarget(combatTarget.transform);

            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

            yield return new WaitUntil(() => !GetComponent<Animator>().IsInTransition(0));

            while (animator.GetCurrentAnimatorStateInfo(0).IsName("Rushing") && 
                !animator.IsInTransition(0) &&
                Vector3.Distance(startingPoint,transform.position) <= RUSH_ATTACK_RADIUS)
            {
                if (isPlayerInAttackRange(savedNormalAttackRadius))
                {
                    break;
                }
                yield return null;
            }

            if (combatSystem.CurrentAttackType != AttackType.Special)
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
        }

    }
}
