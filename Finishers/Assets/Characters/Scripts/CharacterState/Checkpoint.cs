using Finisher.Characters;
using Finisher.Characters.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacterController playerController = other.GetComponent<PlayerCharacterController>();
        if (playerController != null)
        {
            playerController.gameObject.GetComponent<CharacterState>().spawnConfig = new SpawnConfig(playerController.transform);
        }
    }
}
