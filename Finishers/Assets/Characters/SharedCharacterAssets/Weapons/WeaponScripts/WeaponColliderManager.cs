using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Weapons
{
    [DisallowMultipleComponent]
    public class WeaponColliderManager : MonoBehaviour
    {
        protected FinisherSystem finisherSystem;
        protected CombatSystem combatSystem;
        protected CharacterState characterState;
        private BoxCollider boxCollider;
        public float soulBonusDamage;
        [HideInInspector] public float currentBonus = 0;

        void Awake()
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
            // Dont hit something in the same layer
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
                        finisherSystem.HitCharacter(targetHealthSystem, soulBonus:currentBonus);
                    }
                    else
                    {
                        combatSystem.HitCharacter(targetHealthSystem, soulBonus: currentBonus);
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
