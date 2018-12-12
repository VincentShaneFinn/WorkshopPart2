using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    public class PlayerCharacterController : HumanCharacterController
    {

        // do some neat stuff if with animation events or something to turn of is turning allowed
        //protected override bool IsTurningAllowed()
        //{
        //    return true;
        //}

        // TODO put something here that only plays animation if we can do it

        public void TryHitAnimation()
        {
            animator.SetTrigger("Attack");
            animator.ResetTrigger("Dodge");
        }

        public void TryDodgeAnimation()
        {
            animator.SetTrigger("Dodge");
            animator.ResetTrigger("Attack");
        }

        //called by an event in the animation
        public void Hit()
        {
            print("hit something now");
        }
    }
}