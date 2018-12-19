using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    public class PlayerCharacterController : CharacterAnimator
    {
        //[Header("Player Controller Specific Settings")]
        private Vector3 movementInputDirection;
        private bool jumpInput;
        private bool runInput;
        //What are some things the player does and the enemies do differently, put here
        
        void FixedUpdate()
        {
            if (dying) { return;}
            if (CanMove || CanRotate)
            {
                moveCharacter(movementInputDirection, jumpInput, runInput);
            }
            else
            {
                moveCharacter(Vector3.zero);
            }
        }

        public void MoveCharacter(Vector3 movementInputDirection, bool jumpInput = false, bool runInput = false)
        {
            this.movementInputDirection = movementInputDirection;
            this.jumpInput = jumpInput;
            this.runInput = runInput;
        }
    }
}