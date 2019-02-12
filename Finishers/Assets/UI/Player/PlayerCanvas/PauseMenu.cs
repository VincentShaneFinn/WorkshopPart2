﻿using UnityEngine;
using UnityEngine.SceneManagement;

using Finisher.Core;

namespace Finisher.UI
{
    public class PauseMenu : MonoBehaviour
    {

        private GameObject PauseMenuObject;

        // Start is called before the first frame update
        void Start()
        {
            PauseMenuObject = GetComponent<PlayerUIObjects>().PauseMenuObject;
            PauseMenuObject.SetActive(false);

            // Lock or unlock the cursor.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            GameManager.instance.GamePaused = false;
            Time.timeScale = 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (FinisherInput.Pause())
            {
                TogglePauseMenu();
            }
        }

        public void TogglePauseMenu()
        {
            var paused = !GameManager.instance.GamePaused;
            GameManager.instance.GamePaused = paused;

            if (paused)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
            }

            PauseMenuObject.SetActive(paused);
            Cursor.visible = paused;
        }

        public void Restart()
        {
            SceneManager.LoadScene(0);
        }

        public void Quit()
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }
    }
}
