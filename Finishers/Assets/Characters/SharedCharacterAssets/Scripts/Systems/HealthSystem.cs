using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Finisher.Characters
{
    [RequireComponent(typeof(CharacterAnimator))]
    public class HealthSystem : MonoBehaviour
    {

        [SerializeField] HealthSystemConfig config;
        private CharacterAnimator characterAnim;
        private Animator animator;
        private AnimatorOverrideController animOverrideController;

        void Start()
        {
            characterAnim = GetComponent<CharacterAnimator>();
            animOverrideController = characterAnim.animOverrideController;
            animator = GetComponent<Animator>();

            Assert.IsNotNull(config);
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.K)) // todo remove testing buttons
            {
                Knockback(config.KnockbackAnimations[UnityEngine.Random.Range(0,config.KnockbackAnimations.Length)]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Kill();
            }
        }

        public void Kill()
        {
            if (characterAnim.Dying) { return; }
            characterAnim.Dying = true;
            animator.SetBool(CharAnimParams.DYING_BOOL, true);
        }

        public void Knockback(AnimationClip animClip)
        {
            animOverrideController[AnimOverrideIndexes.KNOCKBACK_INDEX] = animClip;
            animator.SetTrigger(CharAnimParams.KNOCKBACK_TRIGGER);
        }
    }
}
