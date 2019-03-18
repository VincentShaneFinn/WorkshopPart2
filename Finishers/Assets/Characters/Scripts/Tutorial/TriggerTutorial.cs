using Finisher.Characters;
using Finisher.Characters.Player;
using Finisher.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTutorial : MonoBehaviour
{
    protected bool isActive = false;
    public GameObject tutorialMessageUI;
    protected CharacterState state = null;

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Time.timeScale = 0;
            GameManager.instance.GamePaused = true;
            if (FinisherInput.Continue())
            {
                Time.timeScale = 1;
                tutorialMessageUI.SetActive(false);
                Destroy(gameObject);
                GameManager.instance.GamePaused = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMoveInputProcessor>() != null)
        {
            isActive = true;
            state = other.GetComponent<CharacterState>();
            tutorialMessageUI.SetActive(true);
            Time.timeScale = 0;
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
