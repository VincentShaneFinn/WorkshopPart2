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
                coroutine = KnightCombatSystem.RushingCoroutine();
                KnightCombatSystem.StartCoroutine(coroutine);
                KnightCombatSystem.StartCoroutine(KnightCombatSystem.MonitorSpecialAttackStatus());
            }
            if (stateInfo.IsName(AnimConstants.States.RUSHING_STATE))
            {
                KnightCombatSystem.StopCoroutine(coroutine);
                KnightCombatSystem.StartCoroutine(coroutine);
            }

        }
        
    }
}