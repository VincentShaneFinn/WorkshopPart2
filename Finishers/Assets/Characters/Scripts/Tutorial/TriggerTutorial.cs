using Finisher.Characters;
using Finisher.Characters.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTutorial : MonoBehaviour
{
    protected bool isActive = false;
    public GameObject tutorialMessageUI;
    public CharacterState state = null;

    // Update is called once per frame
    void Update()
    {
        if (isActive && FinisherInput.Continue())
        {
            Time.timeScale = 1;
            tutorialMessageUI.SetActive(false);
            Destroy(gameObject);
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
        }
        Time.timeScale = 1;
    }
}
