using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    public class PlayerCharacterController : HumanCharacterController
    {
        [HideInInspector] public bool canPerformNextAction = true;

        void Start()
        {
            base.Initialization();
            canPerformNextAction = true;
        }

        // TODO replace triggers with something else? or at least don't set the trigger, or erase the trigger if it should not be qued
        public void TryHitAnimation()
        {
            canPerformNextAction = false;
            Invoke("ActionBegun", .1f);
            animator.SetTrigger("Attack");
            animator.ResetTrigger("Dodge");
        }

        public void TryDodgeAnimation()
        {
            canPerformNextAction = false;
            SetInstantCharacterRotation();
            Invoke("ActionBegun", .1f);
            animator.SetTrigger("Dodge");
            animator.ResetTrigger("Attack");
        }

        // TODO find a better system then animation events, especially for action completed, since that doesnt get called if the animation can be interuped by transition blend
        //called by an event in the animation
        public void Hit()
        {
            //ActionCompleted();
            print("hit something now");
        }

        public void ActionCompleted()
        {
            print("action completed");
            canPerformNextAction = true;
            RestoreRotationLerp();
        }

        public void ActionBegun()
        {
            LockCharacterRotation();
        }
    }
}