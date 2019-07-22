﻿using Finisher.Characters.Player.Systems;
using UnityEngine;

namespace Finisher.Characters
{
    [DisallowMultipleComponent]
    public class CharAnimStateHandler : MonoBehaviour
    {
        private Animator animator;
        private CharacterAnimator character;
        private CharacterState characterState;
        private PlayerCombatSystem playerCombatSystem;

        private void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<CharacterAnimator>();
            characterState = GetComponent<CharacterState>();
            playerCombatSystem = GetComponent<PlayerCombatSystem>();
        }

        private void Update()
        {
            SetCharacterMovementVariables();
        }

        private void SetCharacterMovementVariables()
        {
            // these two are in code states becuase they can be in multuple animation states,
            // like grabbing an enemy and stabbing, or staggered but knockedback, returning to locomotion then go to staggered
            if (characterState.Grabbing ||
                characterState.Stunned)
            {
                character.CanMove = false;
                character.CanRotate = false;
                return;
            }

            if (playerCombatSystem != null && playerCombatSystem.holdingDummy != null)
            {
                character.CanMove = false;
                return;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.LOCOMOTION_TAG))
            {
                character.CanMove = true;
                character.CanRotate = true;
            }
            else if (animator.IsInTransition(0) && characterState.Attacking)
            {
                character.CanMove = true;
                character.CanRotate = true;
            }
            else
            {
                character.CanRotate = false;
                character.CanMove = false;
            }

            //if (!animator.IsInTransition(0))
            //{
            //    if (characterAnim.Attacking ||
            //        characterAnim.Grabbing ||
            //        animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.UNINTERUPTABLE_TAG))
            //    {
            //        characterAnim.CanRotate = false;
            //        characterAnim.CanMove = false;
            //    }
            //}
            //else
            //{
            //    characterAnim.CanMove = true;
            //    characterAnim.CanRotate = true;
            //}
        }
    }
}