using UnityEngine;
using UnityEngine.SceneManagement;

using Finisher.Core;

namespace Finisher.UI
{
    public class PauseMenu : MonoBehaviour
    {

        private GameObject PauseMenuObject;
        private GameObject ControlMenuObject;
        private GameObject LeftUpperObject;
        private GameObject LeftLowerObject;

        // Start is called before the first frame update
        void Start()
        {
            PauseMenuObject = GetComponent<PlayerUIObjects>().PauseMenuObject;
            PauseMenuObject.SetActive(false);
            ControlMenuObject = GetComponent<PlayerUIObjects>().ControlMenuObject;
            ControlMenuObject.SetActive(false);
            LeftUpperObject = GetComponent<PlayerUIObjects>().LeftUpperObject;
            LeftUpperObject.SetActive(true);
            LeftLowerObject = GetComponent<PlayerUIObjects>().LeftUpperObject;
            LeftLowerObject.SetActive(true);

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
            ControlMenuObject.SetActive(false);
            Cursor.visible = paused;
        }

        public void ToggleControlMenu()
        {
            ControlMenuObject.SetActive(true);
            PauseMenuObject.SetActive(false);
            LeftUpperObject.SetActive(false);
            LeftLowerObject.SetActive(false);
        }

        public void TogglePCControl()
        {
            Debug.Log("Pc selected");
        }
        public void ToggleContollerControl()
        {
            Debug.Log("Controller selected");
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
