using UnityEngine;
using UnityEngine.SceneManagement;

using Finisher.Core;

namespace Finisher.UI
{
    public class ControlMenu : MonoBehaviour
    {

        private GameObject PauseMenuObject;
        private GameObject ControlMenuObject;
        private GameObject LeftUpperObject;
        private GameObject LeftLowerObject;

        private bool controlMenuOpen;

        // Start is called before the first frame update
        void Start()
        {
            PauseMenuObject = GetComponent<PlayerUIObjects>().PauseMenuObject;
            PauseMenuObject.SetActive(false);
            ControlMenuObject = GetComponent<PlayerUIObjects>().ControlMenuObject;
            ControlMenuObject.SetActive(false);
            LeftUpperObject = GetComponent<PlayerUIObjects>().LeftUpperObject;
            LeftUpperObject.SetActive(true);
            LeftLowerObject = GetComponent<PlayerUIObjects>().LeftLowerObject;
            LeftLowerObject.SetActive(true);
            controlMenuOpen = false;
        }

        public void ToggleControlMenu()
        {
            controlMenuOpen = !controlMenuOpen;

            if (controlMenuOpen)
            {

                PauseMenuObject.SetActive(false);
                LeftUpperObject.SetActive(false);
                LeftLowerObject.SetActive(false);
            }
            else
            {
                LeftUpperObject.SetActive(true);
                LeftLowerObject.SetActive(true);
            }

            ControlMenuObject.SetActive(controlMenuOpen);
        }

        public void TogglePrimaryControl()
        {
            Debug.Log("Primary controls selected");
        }
        public void ToggleFinisherControl()
        {
            Debug.Log("Finisher Controls selected");
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
    }
}