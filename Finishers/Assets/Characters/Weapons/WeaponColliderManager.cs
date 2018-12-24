using System.Collections;
using UnityEngine;

namespace Finisher.Characters.Weapons
{
    [DisallowMultipleComponent]
    public class WeaponColliderManager : MonoBehaviour
    {

        [Tooltip("This is a timer that puts a freeze time on both you and the target you hit")]
        [SerializeField] float ImpactFrameTime = .01f;

        private bool isPlayer = false;

        private CombatSystem combatSystem;
        private BoxCollider boxCollider; // todo, add this and the rigidbody

        void Start()
        {
            Initialization();
        }

        private void Initialization()
        {
            combatSystem = GetComponentInParent<CombatSystem>();
            combatSystem.OnDamageFrameChanged += ToggleTriggerCollider;

            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;

            if (combatSystem.gameObject.tag == "Player")
            {
                isPlayer = true;
            }
        }


        void OnTriggerEnter(Collider collider)
        {
            if(collider.gameObject.layer == combatSystem.gameObject.layer)
                return; 

            HealthSystem targetHealthSystem = collider.gameObject.GetComponent<HealthSystem>();
            if (targetHealthSystem && !targetHealthSystem.CharacterAnim.Dying) {
                targetHealthSystem.Damage(combatSystem.currentWeaponDamage);

                if (isPlayer)
                {
                    StartCoroutine(ImpactFrames(targetHealthSystem));
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

        IEnumerator ImpactFrames(HealthSystem targetHealthSystem)
        {
            combatSystem.Animator.speed = 0;
            targetHealthSystem.Animator.speed = 0;

            yield return new WaitForSeconds(ImpactFrameTime);

            combatSystem.Animator.speed = 1;
            targetHealthSystem.Animator.speed = 1;
        }
    }
}
