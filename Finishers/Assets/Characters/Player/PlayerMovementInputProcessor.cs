using System;
using UnityEngine;

namespace Finisher.Characters
{
    [RequireComponent(typeof (CharacterController))]
    public class PlayerMovementInputProcessor : MonoBehaviour
    {
        private CharacterController character; // A reference to the ThirdPersonCharacter on the object
        private Transform cam;                  // A reference to the main camera in the scenes transform
        private Vector3 camForward;             // The current forward direction of the camera
        private Vector3 moveDirection;          // the world-relative desired move direction, calculated from the camForward and user input.
        private bool jump;                      

        
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            character = GetComponent<CharacterController>();
        }


        private void Update()
        {
            if (!jump)
            {
                jump = Input.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (cam != null)
            {
                // calculate camera relative direction to move:
                camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
                moveDirection = v*camForward + h*cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                moveDirection = v*Vector3.forward + h*Vector3.right;
            }

            // TODO add a way to run, and move walking to something else on pc, or make it feel better
            //if (Input.GetKey(KeyCode.LeftShift)) moveDirection *= 0.5f;
            bool running = false;
            if (Input.GetKey(KeyCode.LeftShift)) running = true;

            // pass all parameters to the character control script
            character.Move(moveDirection, crouch, jump, running);
            jump = false;
        }
    }
}
