using UnityEngine;

namespace Finisher.Characters.Weapons
{
    public class Weapon : MonoBehaviour
    {

        [SerializeField] private GameObject soulEffect;

        private WeaponColliderManager colliderManager;

        // Start is called before the first frame update
        void Start()
        {
            colliderManager = GetComponent<WeaponColliderManager>();
        }

        public void ToggleSoul(bool enabled)
        {
            if (enabled)
            {
                colliderManager.currentBonus = colliderManager.soulBonusDamage;
            }
            else
            {
                colliderManager.currentBonus = 0;
            }

            soulEffect.gameObject.SetActive(enabled);
        }

        public void soulOn()
        {
            soulEffect.gameObject.SetActive(true);
            colliderManager.currentBonus = colliderManager.soulBonusDamage;
        }

        public void soulOff()
        {
            soulEffect.gameObject.SetActive(false);
            colliderManager.currentBonus = 0;
        }
    }
}
