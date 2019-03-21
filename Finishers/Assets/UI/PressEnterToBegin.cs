using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressEnterToBegin : MonoBehaviour
{
    [SerializeField] private int sceneToLoad;

    // Update is called once per frame
    void Update()
    {
        if (FinisherInput.ReloadScene())
        {
            LoadSceneByIndex(sceneToLoad);
        }
    }

    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(1);
    }

    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
