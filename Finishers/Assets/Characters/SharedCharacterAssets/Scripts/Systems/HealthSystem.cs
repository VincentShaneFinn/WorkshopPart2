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

        [HideInInspector] public CharacterAnimator character;
        [HideInInspector] public Animator Animator;

        void Start()
        {
            character = GetComponent<CharacterAnimator>();

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
            if (character.Dying) { return; }
            currentHealth = 0;
            healthSlider.value = getCurrentHealthAsPercent();
            character.Dying = true;
            character.SetBoolOverride(AnimContstants.Parameters.DYING_BOOL, true, AnimContstants.OverrideIndexes.DEATH_INDEX, animClip);
        }

        public void Knockback(AnimationClip animClip)
        {
            character.SetTriggerOverride(AnimContstants.Parameters.KNOCKBACK_TRIGGER, AnimContstants.OverrideIndexes.KNOCKBACK_INDEX, animClip);
        }

        private float getCurrentHealthAsPercent()
        {
            return currentHealth / maxHealth;
        }
    }
}
