using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Finisher.Characters.Weapons
{
    [DisallowMultipleComponent]
    public class SoulSword : WeaponColliderManager
    {
        [SerializeField] private CoreCombatDamageSystem soulDamage;
        private GameObject player;

        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.layer == combatSystem.gameObject.layer)
                return;

            HealthSystem targetHealthSystem = collider.gameObject.GetComponent<HealthSystem>();
            CharacterState targetState = collider.gameObject.GetComponent<CharacterState>();

            if (targetHealthSystem)
            {
                if (!targetState.Dying && !targetState.Invulnerable)
                {
                    soulDamage.HitCharacter(combatSystem.gameObject,targetHealthSystem);
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
    }
}
