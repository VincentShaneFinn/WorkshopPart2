using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressEnterToBegin : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    // Update is called once per frame
    void Update()
    {
        if (FinisherInput.ReloadScene())
        {
            LoadSceneByName(sceneToLoad);
        }
    }

    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
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
