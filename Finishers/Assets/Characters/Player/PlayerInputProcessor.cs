using System;
using UnityEngine;

namespace Finisher.Characters
{
    [RequireComponent(typeof (HumanCharacterController))]
    public class PlayerInputProcessor : MonoBehaviour
    {
        private PlayerCharacterController character; // A reference to the ThirdPersonCharacter on the object
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
                Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            character = GetComponent<PlayerCharacterController>();
        }

        [SerializeField] Vector3 TestForce; // TODO remove this 
        private void Update()
        {
            if (!jump)
            {
                jump = Input.GetButtonDown("Jump");
            }

            //testing new animations
            if (Input.GetKeyDown(KeyCode.Mouse0)){
                character.TryHitAnimation();
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                character.TryDodgeAnimation();
            }
            // TODO remove this
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GetComponent<Rigidbody>().AddForce(TestForce, ForceMode.Impulse);
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
