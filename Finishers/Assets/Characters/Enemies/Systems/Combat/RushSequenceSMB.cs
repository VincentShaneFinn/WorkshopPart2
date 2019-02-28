using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Systems
{
    public class RushSequenceSMB : StateMachineBehaviour
    {

        [HideInInspector] public KnightCombatSystem KnightCombatSystem;
        IEnumerator coroutine;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            if (stateInfo.IsName(AnimConstants.States.RUSHING_SETUP_STATE))
            {
                KnightCombatSystem.StartCoroutine(KnightCombatSystem.MonitorSpecialAttackStatus());
                coroutine = KnightCombatSystem.RushingCoroutine();
                KnightCombatSystem.StartCoroutine(coroutine);
            }
            //if (stateInfo.IsName(AnimConstants.States.RUSHING_STATE))
            //{
            //    KnightCombatSystem.StopCoroutine(coroutine);
            //    KnightCombatSystem.StartCoroutine(coroutine);
            //}

        }
        
    }
}