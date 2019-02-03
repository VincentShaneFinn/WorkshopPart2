using UnityEngine;

using Finisher.Characters.Systems;
using System.Collections;

namespace Finisher.Characters.Weapons
{
    public class Weapon : MonoBehaviour
    {

        [SerializeField] private GameObject soulEffect;
        [SerializeField] private ParticleSystem trailEffect;

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

        private void ToggleTrailingEffect(bool fuck)
        {
            print(fuck);
            if (fuck)
            {
                trailEffect.Play();
                time = Time.time;
                StartCoroutine(stopPlayer());
            }
            else
            {
                //StartCoroutine(stopPlayer());
            }

        }

        IEnumerator stopPlayer()
        {
            yield return new WaitForSeconds(.01f);
            trailEffect.Stop();
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
