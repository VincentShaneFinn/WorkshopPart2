using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Weapons
{
    public class WeaponColliderManager : MonoBehaviour
    {
        bool isEnemy;
        bool dontHit = false;

        const float RESTORE_HIT_TIME = .01f; // used to protect from In Out and back In issues
        // todo find a better way to protect a recently hit target

        CombatSystem combatSystem;
        [HideInInspector] public BoxCollider boxCollider; // todo, add this and the rigidbody

        void Start()
        {
            combatSystem = GetComponentInParent<CombatSystem>();
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;
            if (combatSystem.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                isEnemy = true;
            }
            else
            {
                isEnemy = false;
            }
            combatSystem.OnDamageFrameChanged += ToggleTriggerCollider;
        }

        void OnTriggerEnter(Collider collision)
        {
            if(collision.gameObject.layer == combatSystem.gameObject.layer || dontHit)
                return; 

            HealthSystem healthSystem;
            if (healthSystem = collision.gameObject.GetComponent<HealthSystem>()){
                healthSystem.Damage(combatSystem.currentWeaponDamage);
                dontHit = true;
                StartCoroutine(restoreAbilityToHit());
            }
        }

        IEnumerator restoreAbilityToHit()
        {
            yield return new WaitForSeconds(RESTORE_HIT_TIME);
            dontHit = false;
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
    }
}
