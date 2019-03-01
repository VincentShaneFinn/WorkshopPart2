using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressEnterToBegin : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (FinisherInput.ReloadScene())
        {
            SceneManager.LoadScene(1);
        }
    }
}
