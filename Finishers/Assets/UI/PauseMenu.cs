using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Finisher.Core;

namespace Finisher.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] GameObject PauseMenuObject;

        // Start is called before the first frame update
        void Start()
        {
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu();
            }
        }

        // todo create a core gamestate where the game is paused
        public void TogglePauseMenu()
        {
            GameManager.instance.GamePaused = !GameManager.instance.GamePaused;

            if (GameManager.instance.GamePaused)
            {
                GameManager.instance.GamePaused = true;

                Time.timeScale = 0;
                PauseMenuObject.SetActive(true);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                GameManager.instance.GamePaused = false;

                Time.timeScale = 1;
                PauseMenuObject.SetActive(false);

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void Restart()
        {
            SceneManager.LoadScene(0);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
