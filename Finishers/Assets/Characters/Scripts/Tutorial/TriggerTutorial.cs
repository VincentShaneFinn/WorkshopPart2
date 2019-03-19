using Finisher.Characters;
using Finisher.Characters.Player;
using Finisher.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTutorial : Tutorial
{
    protected CharacterState state = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMoveInputProcessor>() != null)
        {
            state = other.GetComponent<CharacterState>();
            showTutorial();
        }
    }

    public void OnDestroy()
    {
        if (state != null)
        {
            state.spawnConfig = new SpawnConfig();
            state.spawnConfig.notDestroyed.Remove(state.spawnConfig.GetFullName(gameObject));
        }
        Time.timeScale = 1;
    }
}
