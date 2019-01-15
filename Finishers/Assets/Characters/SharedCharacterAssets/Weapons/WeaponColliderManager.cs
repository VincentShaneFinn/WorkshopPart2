using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Weapons
{
    [DisallowMultipleComponent]
    public class WeaponColliderManager : MonoBehaviour
    {
        FinisherSystem finisherSystem;
        CombatSystem combatSystem;
        private BoxCollider boxCollider;

        public delegate void HitCharacterDelegete(HealthSystem targetHealthSystem);
        public event HitCharacterDelegete OnHitCharacter;
        public void CallHitCharacterEvent(HealthSystem targetHealthSystem)
        {
            if (OnHitCharacter != null)
            {
                OnHitCharacter(targetHealthSystem);
            }
        }

        void Start()
        {
            Initialization();
        }

        private void Initialization()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;

            subscribeToDelegates();
        }

        void subscribeToDelegates()
        {
            combatSystem = GetComponentInParent<CombatSystem>();
            if (combatSystem)
            {
                combatSystem.OnDamageFrameChanged += ToggleTriggerCollider;

                OnHitCharacter += combatSystem.HitCharacter;
            }

            finisherSystem = GetComponentInParent<FinisherSystem>();
            if (finisherSystem)
            {
                OnHitCharacter += finisherSystem.HitCharacter;
            }
        }

        void OnDestroy()
        {
            if (combatSystem)
            {
                combatSystem.OnDamageFrameChanged -= ToggleTriggerCollider;

                OnHitCharacter -= combatSystem.HitCharacter;
            }

            if (finisherSystem)
            {
                OnHitCharacter -= finisherSystem.HitCharacter;
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            // Dont hit somethting in the same layer
            if (collider.gameObject.layer == combatSystem.gameObject.layer)
                return;

            HealthSystem targetHealthSystem = collider.gameObject.GetComponent<HealthSystem>();
            CharacterState targetState = collider.gameObject.GetComponent<CharacterState>();
            if (targetHealthSystem && !targetState.Dying && !targetState.Invulnerable)
            {
                CallHitCharacterEvent(targetHealthSystem);
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
