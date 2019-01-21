using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Weapons
{
    [DisallowMultipleComponent]
    public class WeaponColliderManager : MonoBehaviour
    {
        FinisherSystem finisherSystem;
        CombatSystem combatSystem;
        CharacterState characterState;
        private BoxCollider boxCollider;

        void Start()
        {
            Initialization();
        }

        private void Initialization()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;

            combatSystem = GetComponentInParent<CombatSystem>();
            finisherSystem = GetComponentInParent<FinisherSystem>();
            characterState = GetComponentInParent<CharacterState>();

            subscribeToDelegates();
        }

        private void subscribeToDelegates()
        {
            combatSystem.OnDamageFrameChanged += ToggleTriggerCollider;
        }

        void OnDestroy()
        {
            combatSystem.OnDamageFrameChanged -= ToggleTriggerCollider;
        }

        void OnTriggerEnter(Collider collider)
        {
            // Dont hit somethting in the same layer
            if (collider.gameObject.layer == combatSystem.gameObject.layer)
                return;

            HealthSystem targetHealthSystem = collider.gameObject.GetComponent<HealthSystem>();
            CharacterState targetState = collider.gameObject.GetComponent<CharacterState>();

            if (targetHealthSystem)
            {
                if(!targetState.Dying && !targetState.Invulnerable)
                {
                    if (characterState.FinisherModeActive)
                    {
                        finisherSystem.HitCharacter(targetHealthSystem);
                    }
                    else
                    {
                        combatSystem.HitCharacter(targetHealthSystem);
                    }
                }
                else if (targetState.IsParryFrame)
                {
                    characterState.Stun(3f, wasParry: true);
                    CombatSystem targetCombatSystem = targetState.GetComponent<CombatSystem>();
                    if (targetCombatSystem)
                    {
                        targetCombatSystem.CallCombatSystemDealtDamageListeners(10f); //TODO: remove magic number
                    }

                    return;
                }
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
