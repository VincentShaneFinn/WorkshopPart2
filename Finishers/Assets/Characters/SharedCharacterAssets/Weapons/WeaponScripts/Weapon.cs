using UnityEngine;

using Finisher.Characters.Systems;
using System.Collections;

namespace Finisher.Characters.Weapons
{
    public class Weapon : MonoBehaviour
    {
        private float time = 0;

        private WeaponColliderManager colliderManager;
        private CombatSystem combatSystem;

        // Start is called before the first frame update
        void Start()
        {
            colliderManager = GetComponent<WeaponColliderManager>();
            combatSystem = GetComponentInParent<CombatSystem>();

            if(combatSystem)
            {
                combatSystem.OnDamageFrameChanged += ToggleTrailingEffect;
            }
        }

        private void OnDestroy()
        {
            if(combatSystem)
            {
                combatSystem.OnDamageFrameChanged -= ToggleTrailingEffect;
            }
        }

        private void ToggleTrailingEffect(bool _enabled)
        {
        }

        public void ToggleSoul(bool enabled)
        {
        }

        public void soulOn()
        {
            colliderManager.currentBonus = colliderManager.soulBonusDamage;
        }

        public void soulOff()
        {
            colliderManager.currentBonus = 0;
        }
    }
}
