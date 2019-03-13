using Finisher.Characters;
using Finisher.Characters.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.UI
{
    public class CombatTargetIndicatorController : MonoBehaviour
    {
        [HideInInspector] public PlayerCharacterController player;
        [SerializeField] private CharacterStateSO playerState;
        [SerializeField] GameObject normalIndicator;
        [SerializeField] GameObject grabButton;
        [SerializeField] GameObject finisherButton;
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            UpdateCombatTargetIndicator(player.CombatTarget != null ? player.CombatTarget.gameObject : null);
        }

        private void UpdateCombatTargetIndicator(GameObject target)
        {
            if (target)
            {
                if (playerState.IsFinisherModeActive && playerState.GetIsCurrentTargetFinishable() && !playerState.IsGrabbing)
                {
                    normalIndicator.SetActive(false);
                    grabButton.SetActive(false);
                    finisherButton.SetActive(true);
                }
                else if (playerState.IsFinisherModeActive && !playerState.IsGrabbing)
                {
                    normalIndicator.SetActive(false);
                    grabButton.SetActive(true);
                    finisherButton.SetActive(false);
                }
                else
                {
                    normalIndicator.SetActive(true);
                    grabButton.SetActive(false);
                    finisherButton.SetActive(false);
                }
                float height = target.GetComponent<CapsuleCollider>().height;
                transform.position = new Vector3(target.transform.position.x, target.transform.position.y +
                    height + (.1f * height), target.transform.position.z);
            }
            else
            {
                normalIndicator.SetActive(false);
                grabButton.SetActive(false);
                finisherButton.SetActive(false);
            }
        }
    }
}