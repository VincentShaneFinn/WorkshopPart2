using UnityEngine;

namespace Finisher.Characters {

    [DisallowMultipleComponent]
    public class CharAnimStateHandler : MonoBehaviour
    {

        private Animator animator;
        private CharacterAnimator characterAnim;

        void Start()
        {
            animator = GetComponent<Animator>();
            characterAnim = GetComponent<CharacterAnimator>();
        }

        void Update()
        {
            SetCharacterMovementVariables();
        }

        private void SetCharacterMovementVariables()
        {
            if (characterAnim.Grabbing ||
                characterAnim.Staggered) 
            {
                characterAnim.CanMove = false;
                characterAnim.CanRotate = false;
                return;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.LOCOMOTION_TAG))
            {
                characterAnim.CanMove = true;
                characterAnim.CanRotate = true;
            }
            else if(animator.IsInTransition(0) && characterAnim.Attacking)
            {
                characterAnim.CanMove = true;
                characterAnim.CanRotate = true;
            }
            else
            {
                characterAnim.CanRotate = false;
                characterAnim.CanMove = false;
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