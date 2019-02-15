using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Systems
{
    public class RushSequenceSMB : StateMachineBehaviour
    {

        public KnightCombatSystem KnightCombatSystem;

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            KnightCombatSystem.StartCoroutine(leftStateMachine(animator));
            KnightCombatSystem.IsPerformingSpecialAttack = true;
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName(AnimConstants.States.RUSHING_ATTACK_STATE))
            {
                KnightCombatSystem.StartCoroutine(KnightCombatSystem.RushingCoroutine());
            }
            if (stateInfo.IsName(AnimConstants.States.RUSHING_STATE))
            {
                KnightCombatSystem.StopAllCoroutines();
                KnightCombatSystem.StartCoroutine(KnightCombatSystem.RushingCoroutine());
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        IEnumerator leftStateMachine(Animator animator)
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.SPECIAL_ATTACK_SEQUENCE_TAG));
            KnightCombatSystem.ResetRushing();
            KnightCombatSystem.IsPerformingSpecialAttack = false;
        }

    }
}