using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    public class PlayerCharacterController : FinisherCharacterController
    {
        [Header("Player Controller Specific Settings")]
        [SerializeField] public bool PlayerCanMove = true; protected override bool CanMove() { return PlayerCanMove; }
        [HideInInspector] public bool canPerformNextAction = true;

        void Start()
        {
            Initialization();
            canPerformNextAction = true;
        }

        // TODO replace triggers with something else? or at least don't set the trigger, or erase the trigger if it should not be qued
        public void TryHitAnimation()
        {
            canPerformNextAction = false;
            // TODO actionbegun is set in the animator. BAD??
            animator.SetTrigger("Attack");
            animator.ResetTrigger("Dodge");
        }

        // TODO fix a bug where the player looks like he is rolling into the wall, need animation that is centered better
        // TODO the velocity needs to be under control on slopes so you dont go flying, like you can sore to the moon on an 80% angle
        // check if OnAnimatorMove velocity is calculating right, otherwise simply deactive root motion?
        public void TryDodgeAnimation()
        {
            canPerformNextAction = false;
            SetInstantCharacterRotation();
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
            PlayerCanMove = true;
            RestoreRotationLerp();
        }

        public void ActionBegun()
        {
            print("action started");
            PlayerCanMove = false;
            LockCharacterRotation();
        }
    }
}