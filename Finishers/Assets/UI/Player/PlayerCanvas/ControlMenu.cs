using Finisher.Cameras;
using Finisher.Core;
using UnityEngine;

namespace Finisher.UI
{
    public class ControlMenu : MonoBehaviour
    {
        public GameObject ControlMenuPrimary;
        public GameObject ControlMenuFinisher;
        private GameObject PauseMenuObject;
        private GameObject ControlMenuObject;
        private GameObject LeftUpperObject;
        private GameObject LeftLowerObject;

        private bool controlMenuOpen;
        private CameraLookController cameraLookController;

        public bool isMusicEnabled
        {
            set
            {
                FindObjectOfType<CharacterMusicHandler>().isMusicEnabled = value;
            }
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
            ControlMenuPrimary.SetActive(true);
            ControlMenuFinisher.SetActive(false);
        }

        public void ToggleFinisherControl()
        {
            ControlMenuPrimary.SetActive(false);
            ControlMenuFinisher.SetActive(true);
        }

        // Start is called before the first frame update
        private void Start()
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

            cameraLookController = FindObjectOfType<CameraLookController>();
        }
    }
}