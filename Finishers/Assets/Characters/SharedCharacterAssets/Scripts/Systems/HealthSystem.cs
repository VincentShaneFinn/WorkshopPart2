using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Finisher.Characters
{

    [RequireComponent(typeof(CharacterAnimator))]
    public class HealthSystem : MonoBehaviour
    {

        [SerializeField] Slider healthSlider;
        [SerializeField] HealthSystemConfig config;
        [SerializeField] float maxHealth = 100;
        [SerializeField] int KnockbackLimit = 2;
        [SerializeField] float FreeKnockbackTime = 1f;

        private float currentHealth;
        private int knockbackCount;

        [HideInInspector] public CharacterAnimator CharacterAnim;
        private Animator animator;
        private AnimatorOverrideController animOverrideController;

        void Start()
        {
            CharacterAnim = GetComponent<CharacterAnimator>();

            animOverrideController = CharacterAnim.animOverrideController;
            animator = GetComponent<Animator>();

            currentHealth = maxHealth;
            healthSlider.value = getCurrentHealthAsPercent();
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.K)) // todo remove testing buttons
            {
                var animIndex = UnityEngine.Random.Range(0, config.KnockbackAnimations.Length);
                Knockback(config.KnockbackAnimations[animIndex]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Damage(10);
            }
        }

        public void Damage(float damage)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(AnimContstants.States.DODGE_STATE)) { return; }
            currentHealth -= damage;
            healthSlider.value = getCurrentHealthAsPercent();

            if (knockbackCount < KnockbackLimit)
            {
                Knockback(config.KnockbackAnimations[UnityEngine.Random.Range(0, config.KnockbackAnimations.Length)]);
                knockbackCount++;
                StartCoroutine(ReleaseCountAfterDelay());
            }

            if(currentHealth <= 0)
            {
                Kill();
            }
        }

        // todo, make this care about consective hits or building up a resistance?
        IEnumerator ReleaseCountAfterDelay()
        {
            yield return new WaitForSeconds(FreeKnockbackTime);
            knockbackCount--;
        }

        public void Kill()
        {
            if (CharacterAnim.Dying) { return; }
            CharacterAnim.Dying = true;
            animator.SetBool(AnimContstants.Parameters.DYING_BOOL, true);
        }

        public void Knockback(AnimationClip animClip)
        {
            animOverrideController[AnimContstants.OverrideIndexes.KNOCKBACK_INDEX] = animClip;
            animator.SetTrigger(AnimContstants.Parameters.KNOCKBACK_TRIGGER);
        }

        private float getCurrentHealthAsPercent()
        {
            return currentHealth / maxHealth;
        }
    }
}
