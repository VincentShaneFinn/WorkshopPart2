using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Weapons
{
    public class WeaponColliderManager : MonoBehaviour
    {

        [Tooltip("This is a timer that puts a freeze time on both you and the target you hit")]
        [SerializeField] float ImpactFrameTime = .01f;

        const float RESTORE_HIT_TIME = .01f; // used to protect from In Out and back In issues
        // todo find a better way to protect a recently hit target

        private float savedSpeedMultiplier;
        private bool dontHit = false;

        private CombatSystem combatSystem;
        private BoxCollider boxCollider; // todo, add this and the rigidbody

        void Start()
        {
            combatSystem = GetComponentInParent<CombatSystem>();
            combatSystem.OnDamageFrameChanged += ToggleTriggerCollider;
            savedSpeedMultiplier = combatSystem.GetComponent<CharacterMotor>().GlobalAnimSpeedMultiplier;

            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;

        }

        void OnTriggerEnter(Collider collision)
        {
            if(collision.gameObject.layer == combatSystem.gameObject.layer || dontHit)
                return; 

            HealthSystem targetHealthSystem = collision.gameObject.GetComponent<HealthSystem>();
            if (targetHealthSystem && !targetHealthSystem.CharacterAnim.Dying) {
                targetHealthSystem.Damage(combatSystem.currentWeaponDamage);

                dontHit = true;
                StartCoroutine(ImpactFrames(targetHealthSystem));
                StartCoroutine(restoreAbilityToHit());
            }
        }

        void ToggleTriggerCollider(bool isDamageFrame)
        {
            if (isDamageFrame)
            {
                boxCollider.enabled = true;
            }
            else
            {
                boxCollider.enabled = false;
            }
        }

        IEnumerator ImpactFrames(HealthSystem targetHealthSystem)
        {
            combatSystem.CharacterAnim.GlobalAnimSpeedMultiplier = 0;
            targetHealthSystem.CharacterAnim.GlobalAnimSpeedMultiplier = 0;
            yield return new WaitForSeconds(ImpactFrameTime);
            combatSystem.CharacterAnim.GlobalAnimSpeedMultiplier = savedSpeedMultiplier;
            targetHealthSystem.CharacterAnim.GlobalAnimSpeedMultiplier = savedSpeedMultiplier;

            StartCoroutine(restoreAbilityToHit());
        }

        IEnumerator restoreAbilityToHit()
        {
            yield return new WaitForSeconds(RESTORE_HIT_TIME);
            dontHit = false;
        }
    }
}
