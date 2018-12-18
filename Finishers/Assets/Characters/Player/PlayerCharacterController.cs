using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    public class PlayerCharacterController : CharacterAnimator
    {
        [Header("Player Controller Specific Settings")]
        [SerializeField] public bool PlayerCanMove = true; protected override bool CanMove() { return PlayerCanMove; }
        [HideInInspector] public bool canPerformNextAction = true;

        void Start()
        {
            canPerformNextAction = true;
        }

        // TODO replace triggers with something else? or at least don't set the trigger, or erase the trigger if it should not be qued
        public void TryHitAnimation()
        {
            //canPerformNextAction = false;
            animator.SetTrigger("Attack");
            animator.ResetTrigger("Dodge");
        }

        // TODO fix a bug where the player looks like he is rolling into the wall, need animation that is centered better
        // TODO the velocity needs to be under control on slopes so you dont go flying, like you can sore to the moon on an 80% angle
        // check if OnAnimatorMove velocity is calculating right, otherwise simply deactive root motion?
        public void TryDodgeAnimation()
        {
            //canPerformNextAction = false;
            //SetInstantCharacterRotation();
            animator.SetTrigger("Dodge");
            animator.ResetTrigger("Attack");
        }

        public void Hit()
        {
            print("hit something now");
        }
    }
}