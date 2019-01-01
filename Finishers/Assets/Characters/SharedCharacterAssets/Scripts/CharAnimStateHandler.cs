using UnityEngine;

namespace Finisher.Characters {

    [DisallowMultipleComponent]
    public class CharAnimStateHandler : MonoBehaviour
    {
        public bool IsAttacking {
            get {
                return animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.ATTACKRIGHT_TAG) ||
                    animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.ATTACKLEFT_TAG);
            }
        }
        public bool IsDodging { get { return animator.GetCurrentAnimatorStateInfo(0).IsName(AnimContstants.States.DODGE_STATE); } }

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
            if (!animator.IsInTransition(0))
            {
                if (IsAttacking ||
                    animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimContstants.Tags.UNINTERUPTABLE_TAG))
                {
                    characterAnim.CanRotate = false;
                    characterAnim.CanMove = false;
                }
            }
            else
            {
                characterAnim.CanMove = true;
                characterAnim.CanRotate = true;
            }
        }

    }
}