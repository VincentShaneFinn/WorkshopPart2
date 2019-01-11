using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Weapons
{
    [DisallowMultipleComponent]
    public class WeaponColliderManager : MonoBehaviour
    {
        private CombatSystem combatSystem;
        private BoxCollider boxCollider;

        void Start()
        {
            Initialization();
        }

        private void Initialization()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;
        }

        void OnEnable()
        {
            combatSystem = GetComponentInParent<CombatSystem>();
            combatSystem.OnDamageFrameChanged += ToggleTriggerCollider;
        }

        void OnDisable()
        {
            combatSystem.OnDamageFrameChanged -= ToggleTriggerCollider;
            StopAllCoroutines();
        }

        void OnTriggerEnter(Collider collider)
        {
            // Dont hit somethting in the same layer
            if (collider.gameObject.layer == combatSystem.gameObject.layer)
                return;

            DamageCharacter(collider.gameObject.GetComponent<HealthSystem>(), collider.gameObject.GetComponent<CharacterState>());
        }

        private void DamageCharacter(HealthSystem targetHealthSystem, CharacterState targetState)
        {
            if (targetHealthSystem && !targetState.Dying && !targetState.Invulnerable)
            {
                targetHealthSystem.DamageHealth(combatSystem.CurrentAttackDamage);

                combatSystem.DealtDamage(targetHealthSystem);
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
    }
}
