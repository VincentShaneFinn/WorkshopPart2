using Finisher.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    protected bool isActive = false;
    public GameObject tutorialMessageUI;
    static bool riposteTutorialReady = true;
    static bool finisherTutorialReady = true;

    void Update()
    {
        if (isActive)
        {
            Time.timeScale = 0;
            GameManager.instance.GamePaused = true;
            if (FinisherInput.Continue() || FinisherInput.LightAttack())
            {
                Time.timeScale = 1;
                tutorialMessageUI.SetActive(false);
                GameManager.instance.GamePaused = false;
                Destroy(gameObject);
            }
        }
    }

    public void showTutorial()
    {
        isActive = true;
        tutorialMessageUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void showRiposteTutorial()
    {
        if (Persistance.GetSingleton().riposteTutorialReady)
        {
            isActive = true;
            tutorialMessageUI.SetActive(true);
            Time.timeScale = 0;
        }
        Persistance.GetSingleton().riposteTutorialReady = false;
    }

    public void showFinisherTutorial()
    {
        if (Persistance.GetSingleton().finisherTutorialReady)
        {
            isActive = true;
            tutorialMessageUI.SetActive(true);
            Time.timeScale = 0;
        }
        Persistance.GetSingleton().finisherTutorialReady = false;
    }
}
