using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    public class PlayerCharacterController : CharacterAnimator
    {
        //[Header("Player Controller Specific Settings")]

        [HideInInspector] public bool UseStraffingTarget = false; // tries to look at the set staffing target if true, matches camera rotation if false

        private Transform cameraTransform;
        private Vector3 movementInputDirection;
        private bool jumpInput;
        private bool runInput;

        void Start()
        {
            GetMainCameraTransform();
        }

        void FixedUpdate()
        {
            if (CanMove || CanRotate) 
            {
                moveCharacter(movementInputDirection, jumpInput, runInput); 
            }
            else
            {
                moveCharacter(Vector3.zero);
            }
        }

        public void MoveCharacter(Vector3 movementInputDirection, bool jumpInput = false, bool runInput = false)
        {
            this.movementInputDirection = movementInputDirection;
            this.jumpInput = jumpInput;
            this.runInput = runInput;
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
                base.setStrafingRotation();
            }
            else
            {
                transform.rotation = cameraTransform.rotation;
            }
        }
        #endregion
    }
}