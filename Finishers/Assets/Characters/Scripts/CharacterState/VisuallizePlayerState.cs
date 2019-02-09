using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters {
    public class VisuallizePlayerState : MonoBehaviour
    {

        CharacterStateSO playerState;

        // Start is called before the first frame update
        void Start()
        {
            var player = GameObject.FindGameObjectWithTag(TagNames.PlayerTag);
            playerState = player.GetComponent<CharacterStateFromSO>().stateSO;
        }

        // Update is called once per frame
        void Update()
        {
            updateVariables();
        }

        private void updateVariables()
        {
            if (playerState)
            {
                print("Health: " + playerState.GetCurrentHealthAsPercentage());
                print("CombatTargetVolatility: " + playerState.GetCombatTargetVolatilityAsPercent());
                print("Attacking: " + playerState.IsAttacking);
            }
        }
    }
}