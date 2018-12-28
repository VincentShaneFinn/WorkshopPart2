using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Finisher.UI;

namespace Finisher.Characters
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterAnimator))]
    public class HealthSystem : MonoBehaviour
    {
        [Tooltip("Player's is set automatically")]
        [SerializeField] Slider healthSlider;
        [SerializeField] HealthSystemConfig config;
        [SerializeField] float maxHealth = 100;
        [SerializeField] int KnockbackLimit = 2;
        [SerializeField] float FreeKnockbackTime = 1f;

        private float currentHealth;
        private int knockbackCount;

        [HideInInspector] public CharacterAnimator CharacterAnim;
        [HideInInspector] public Animator Animator;
        private AnimatorOverrideController animOverrideController;

        void Start()
        {
            CharacterAnim = GetComponent<CharacterAnimator>();

            animOverrideController = new AnimatorOverrideController(CharacterAnim.animOverrideController);
            Animator = GetComponent<Animator>();

            currentHealth = maxHealth;

            GetHealthSlider();
            healthSlider.value = getCurrentHealthAsPercent();
        }

        void GetHealthSlider()
        {
            if(gameObject.tag == "Player")
            {
                healthSlider = FindObjectOfType<PlayerUIObjects>().HealthSlider;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Damage(10);
            }
        }

        public void Damage(float damage)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimContstants.States.DODGE_STATE)) { return; }
            currentHealth -= damage;
            healthSlider.value = getCurrentHealthAsPercent();

            if (knockbackCount < KnockbackLimit)
            {
                // todo make a unique random number generator so they dont all die the same way each frame?
                Knockback(config.KnockbackAnimations[UnityEngine.Random.Range(0, config.KnockbackAnimations.Length)]); 
                knockbackCount++;
                StartCoroutine(ReleaseCountAfterDelay());
            }

            if(currentHealth <= 0)
            {
                Kill(config.NormalDeathAnimations[UnityEngine.Random.Range(0,config.NormalDeathAnimations.Length)]);
            }
        }

        // todo, make this care about consective hits or building up a resistance?
        IEnumerator ReleaseCountAfterDelay()
        {
            yield return new WaitForSeconds(FreeKnockbackTime);
            knockbackCount--;
        }

        public void Kill(AnimationClip animClip)
        {
            if (CharacterAnim.Dying) { return; }
            animOverrideController[AnimContstants.OverrideIndexes.DEATH_INDEX] = animClip;
            Animator.runtimeAnimatorController = animOverrideController;
            currentHealth = 0;
            healthSlider.value = getCurrentHealthAsPercent();
            CharacterAnim.Dying = true;
            Animator.SetBool(AnimContstants.Parameters.DYING_BOOL, true);
        }

        public void Knockback(AnimationClip animClip)
        {
            animOverrideController[AnimContstants.OverrideIndexes.KNOCKBACK_INDEX] = animClip;
            Animator.runtimeAnimatorController = animOverrideController;
            Animator.SetTrigger(AnimContstants.Parameters.KNOCKBACK_TRIGGER);
        }

        private float getCurrentHealthAsPercent()
        {
            return currentHealth / maxHealth;
        }
    }
}
