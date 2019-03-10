using UnityEngine;

namespace Finisher.UI
{
    public class ChangeButtonIcon : MonoBehaviour
    {


        public GameObject PS4Icon;
        public GameObject PCIcon;

        // Use this for initialization
        void Start()
        {
            PS4Icon.SetActive(false);
            PCIcon.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (ControlMethodDetector.GetCurrentControlType() == ControlType.PC)
            {
                PS4Icon.SetActive(false);
                PCIcon.SetActive(true);
            }
            else if (ControlMethodDetector.GetCurrentControlType() == ControlType.PS4)
            {
                PS4Icon.SetActive(true);
                PCIcon.SetActive(false);
            }
        }
    }
}