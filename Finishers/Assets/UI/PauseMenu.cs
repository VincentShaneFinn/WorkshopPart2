using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Finisher.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] GameObject PauseMenuObject;

        bool gamePaused = false; // Whether the cursor should be hidden and locked.

        // Start is called before the first frame update
        void Start()
        {
            PauseMenuObject.SetActive(false);

            gamePaused = false;
            // Lock or unlock the cursor.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
            gamePaused = !gamePaused;

            if (gamePaused)
            {
                Time.timeScale = 0;
                PauseMenuObject.SetActive(true);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
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
