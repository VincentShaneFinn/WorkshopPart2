using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Enemies
{
    public class EnemyKnightAI : EnemyAI
    {

        private float savedNormalAttackRadius;

        private bool useRushAttack = false;
        private const float RUSH_ATTACK_RADIUS = 6f;
        
        protected override void Start()
        {
            base.Start();

            savedNormalAttackRadius = attackRadius;

            setContext();
        }

        private void setContext()
        {
            attackRadius = RUSH_ATTACK_RADIUS;
            useRushAttack = true;
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

        IEnumerator RushAttackSequence()
        {
            GetComponent<Animator>().SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
            GetComponent<Animator>().SetInteger("SpecialAttackIndex", 1);

            yield return null;

            yield return new WaitUntil(() => !GetComponent<Animator>().IsInTransition(0));

            GetComponent<Animator>().SetInteger("SpecialAttackIndex", 0);

            while (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("RushSetup"))
            {
                yield return null;
            }

            print("Setup Over");

            yield return new WaitUntil(() => !GetComponent<Animator>().IsInTransition(0));

            while (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Rushing"))
            {
                if (isPlayerInAttackRange(3f))
                {
                    break;
                }
                yield return null;
            }

            print("Rushing Over");
            GetComponent<Animator>().SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);

            yield return new WaitUntil(() => !GetComponent<Animator>().IsInTransition(0));

            while (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("RushAttack"))
            {
                yield return null;
            }

            print("Sequence Over");
        }

    }
}
