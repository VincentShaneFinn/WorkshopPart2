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
        private FinisherSystem finisherSystem;
        private BoxCollider boxCollider;

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
                finisherSystem = GetComponentInParent<FinisherSystem>();
                isPlayer = true;
            }

        }

        void OnTriggerEnter(Collider collider)
        {
            // Dont hit somethting in the same layer
            if (collider.gameObject.layer == combatSystem.gameObject.layer)
                return;

            DamageCharacter(collider.gameObject.GetComponent<HealthSystem>());
        }

        private void DamageCharacter(HealthSystem targetHealthSystem)
        {
            if (targetHealthSystem && !targetHealthSystem.character.Dying)
            {
                targetHealthSystem.DamageHealth(combatSystem.CurrentAttackDamage);

                if (finisherSystem)
                {
                    if (finisherSystem.FinisherModeActive)
                    {
                        finisherSystem.StabbedEnemy(targetHealthSystem.gameObject);
                    }
                    else
                    {
                        if (!finisherSystem.FinisherModeActive)
                        {
                            finisherSystem.GainFinisherMeter(finisherSystem.CurrentFinisherGain);
                        }
                    }
                }

                if (isPlayer)
                {
                    if (finisherSystem.FinisherModeActive)
                    {
                        targetHealthSystem.DamageVolatility(finisherSystem.CurrentVolatilityDamage);
                    }
                    StartCoroutine(ImpactFrames(targetHealthSystem));
                }
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
