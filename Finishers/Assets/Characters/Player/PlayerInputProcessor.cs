using System;
using System.Collections;
using UnityEngine;

namespace Finisher.Characters
{
    [RequireComponent(typeof (PlayerCharacterController))]
    public class PlayerInputProcessor : MonoBehaviour
    {
        #region member variables
        [Tooltip("The amount of time that to keep an input in the que")]
        [SerializeField] float rememberInputForSeconds = .4f;

        private PlayerCharacterController character = null; // A reference to the ThirdPersonCharacter on the object
        private Transform camRig = null;                  // A reference to the main camera in the scenes transform
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
            // get the third person character ( this should never be null due to require component )
            character = GetComponent<PlayerCharacterController>();

            // get the transform of the main camera
            camRig = character.GetMainCameraTransform();
        }

        private void Update()
        {
            GetJumpInput();
            if (character.IsGrounded)
            {
                SetNextInput();
            }
            UseNextInput();

            TestingInputZone();
        }

        private void TestingInputZone()
        {
            // todo remove this testing code
            // todo also consider allowing can move to be public
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                character.CanMove = false;
            }
            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                character.CanMove = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                character.CanRotate = false;
            }
            if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                character.CanRotate = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                character.Kill();
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
                    print("attack");
                    character.Attack();
                    break;
                case DODGE:
                    character.Dodge();
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

        void FixedUpdate()
        {
            if (character.CanMove || character.CanRotate)
            {
                ProcessMovementInput();
            }

            jump = false;
        }

        private void ProcessMovementInput()
        {
            // read inputs
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            // calculate move direction to pass to character
            if (camRig != null)
            {
                // calculate camera relative direction to move:
                camForward = Vector3.Scale(camRig.forward, new Vector3(1, 0, 1)).normalized;
                moveDirection = v * camForward + h * camRig.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                moveDirection = v * Vector3.forward + h * Vector3.right;
            }

            //use to be how walking was done, running may need a small rework
            //if (Input.GetKey(KeyCode.LeftShift)) moveDirection *= 0.5f;

            // pass all parameters to the character control script
            character.MoveCharacter(moveDirection, jump, Input.GetKey(KeyCode.LeftShift));//change to use run button
        }
    }
}
