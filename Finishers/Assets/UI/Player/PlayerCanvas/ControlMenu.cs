using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Finisher.Cameras;
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

        public GameObject ControlMenuPrimary;
        public GameObject ControlMenuFinisher;

        private CameraLookController cameraLookController;
        public Slider sensitivitySlider;

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

            cameraLookController = FindObjectOfType<CameraLookController>();
            sensitivitySlider.value = cameraLookController.turnSpeed;
            sensitivitySlider.onValueChanged.AddListener(delegate { updateSensitivity(); });
        }

        void updateSensitivity()
        {
            cameraLookController.turnSpeed = sensitivitySlider.value;
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

    }
}