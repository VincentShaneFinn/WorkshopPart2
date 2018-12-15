using System;
using System.Collections;
using UnityEngine;

namespace Finisher.Characters
{
    [RequireComponent(typeof (PlayerCharacterController))]
    public class PlayerInputProcessor : MonoBehaviour
    {
        #region member variables
        [SerializeField] float rememberInputForSeconds = .4f;

        private PlayerCharacterController character = null; // A reference to the ThirdPersonCharacter on the object
        private Transform cam = null;                  // A reference to the main camera in the scenes transform
        private Vector3 camForward;             // The current forward direction of the camera
        private Vector3 moveDirection;          // the world-relative desired move direction, calculated from the camForward and user input.
        private bool jump = false;
        private String nextInput = "";
        private String previousInput = "";
        private float lastInputTime = 0;

        const string PRIMARY_ATTACK = "Mouse 0";
        const string DODGE = "Mouse 1";
        #endregion

        private void Start()
        {
            // get the transform of the main camera
            GetMainCameraTransform();

            // get the third person character ( this should never be null due to require component )
            character = GetComponent<PlayerCharacterController>();
        }

        private void GetMainCameraTransform()
        {
            if (Camera.main != null)
            {
                cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }
        }

        private void Update()
        {
            GetJumpInput();
            if (character.GetIsGrounded())
            {
                SetNextInput();
            }
            if (character.canPerformNextAction)
            {
                UseNextInput();
            }
        }

        private void SetNextInput()
        {
            //testing new animations
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                nextInput = PRIMARY_ATTACK;
                lastInputTime = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if(previousInput != DODGE)
                    nextInput = DODGE;
                lastInputTime = Time.time;
            }
            else
            {
                if(Time.time - lastInputTime > rememberInputForSeconds)
                {
                    nextInput = "";
                }
            }
        }

        private void UseNextInput()
        {
            switch (nextInput)
            {
                case PRIMARY_ATTACK:
                    character.TryHitAnimation();
                    break;
                case DODGE:
                    character.TryDodgeAnimation();
                    break;
            }
            previousInput = nextInput;
            nextInput = "";
        }



        private void GetJumpInput()
        {
            if (!jump)
            {
                jump = Input.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            ProcessMovementInput();
        }

        private void ProcessMovementInput()
        {
            // read inputs
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            // calculate move direction to pass to character
            if (cam != null)
            {
                // calculate camera relative direction to move:
                camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
                moveDirection = v * camForward + h * cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                moveDirection = v * Vector3.forward + h * Vector3.right;
            }

            //use to be how walking was done, running may need a small rework
            //if (Input.GetKey(KeyCode.LeftShift)) moveDirection *= 0.5f;

            // pass all parameters to the character control script
            character.Move(moveDirection, jump, Input.GetKey(KeyCode.LeftShift));//change to use run button
            jump = false;
        }
    }
}
