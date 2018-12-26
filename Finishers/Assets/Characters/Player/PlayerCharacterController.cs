using UnityEngine;

using Finisher.Cameras;

namespace Finisher.Characters
{
    public class PlayerCharacterController : CharacterAnimator
    {
        //[Header("Player Controller Specific Settings")]

        [HideInInspector] public bool UseStraffingTarget = false; // tries to look at the set staffing target if true, matches camera rotation if false

        private Transform cameraTransform;
        private FreeLookCam freeLookCam;

        void Start()
        {
            GetMainCameraTransform();
            freeLookCam = cameraTransform.GetComponent<FreeLookCam>();
            freeLookCam.playerCharacter = this;
        }

        #region CameraGetter and Strafing Match Camera

        public Transform GetMainCameraTransform()
        {
            if (GameObject.FindObjectOfType<Finisher.Cameras.FreeLookCam>())
            {
                cameraTransform = GameObject.FindObjectOfType<Finisher.Cameras.FreeLookCam>().gameObject.transform;
            }
            else if (Camera.main != null)
            {
                Debug.Log("Non Game Camera In Use");
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }
            return cameraTransform;
        }

        protected override void setStrafingRotation()
        {
            if (UseStraffingTarget)
            {
                freeLookCam.LookAtTarget = CurrentLookTarget;
                transform.rotation = cameraTransform.rotation;
            }
            else
            {
                freeLookCam.LookAtTarget = null;
                transform.rotation = cameraTransform.localRotation;
            }
        }
        #endregion
    }
}