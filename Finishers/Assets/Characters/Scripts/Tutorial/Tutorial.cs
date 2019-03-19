using Finisher.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    protected bool isActive = false;
    public GameObject tutorialMessageUI;

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
                GameManager.instance.GamePaused = false;
                Destroy(gameObject);
            }
        }
    }

    public virtual void showTutorial()
    {
        isActive = true;
        tutorialMessageUI.SetActive(true);
        Time.timeScale = 0;
    }
}
