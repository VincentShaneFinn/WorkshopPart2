using Finisher.Core;
using System;
using System.Collections;
using UnityEngine;

namespace Finisher.Characters
{
    [RequireComponent(typeof (PlayerCharacterController))]
    public class PlayerInputProcessor : MonoBehaviour
    {
        #region member variables

        private PlayerCharacterController character = null; // A reference to the ThirdPersonCharacter on the object
        private CombatSystem combatSystem;
        private Transform camRig = null;                  // A reference to the main camera in the scenes transform
        private Vector3 camForward;             // The current forward direction of the camera
        private Vector3 moveDirection;          // the world-relative desired move direction, calculated from the camForward and user input.
        private bool jump = false;

        const string PRIMARY_ATTACK = "Mouse 0";
        const string DODGE = "Mouse 1";

        #endregion

        private void Start()
        {
            // get the third person character ( this should never be null due to require component )
            character = GetComponent<PlayerCharacterController>();
            combatSystem = GetComponent<CombatSystem>();

            // get the transform of the main camera
            camRig = character.GetMainCameraTransform();
        }

        private void Update()
        {
            if (GameManager.instance.GamePaused) { return; }


            if (character.isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    combatSystem.LightAttack();
                }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    combatSystem.HeavyAttack();
                }

                if (Input.GetKeyDown(KeyCode.Mouse3) || Input.GetKeyDown(KeyCode.LeftControl))
                {
                    var moveDirection = GetMoveDirection();
                    combatSystem.Dodge(moveDirection);
                }
            }

            TestingInputZone();
        }

        // todo, make sure it gets the direction with respect to the camera rig rotation
        MoveDirection GetMoveDirection()
        {
            float horizonatal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if(Mathf.Abs(vertical) >= Mathf.Abs(horizonatal)) // forward backward carries more weight
            {
                if(vertical >= 0)
                {
                    return MoveDirection.Forward;
                }
                else
                {
                    return MoveDirection.Backward;
                }
            }
            else
            {
                if (horizonatal >= 0)
                {
                    return MoveDirection.Right;
                }
                else
                {
                    return MoveDirection.Left;
                }
            }
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
            else
            {
                character.MoveCharacter(Vector3.zero);
            }

            jump = false;
        }

        private void ProcessMovementInput()
        {
            // read inputs
            float horizonatal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // calculate move direction to pass to character
            if (camRig != null)
            {
                // calculate camera relative direction to move:
                camForward = Vector3.Scale(camRig.forward, new Vector3(1, 0, 1)).normalized;
                moveDirection = vertical * camForward + horizonatal * camRig.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                moveDirection = vertical * Vector3.forward + horizonatal * Vector3.right;
            }

            //use to be how walking was done, running may need a small rework
            //if (Input.GetKey(KeyCode.LeftShift)) moveDirection *= 0.5f;

            // pass all parameters to the character control script
            character.MoveCharacter(moveDirection, jump, Input.GetKey(KeyCode.LeftShift));//change to use run button
        }
    }
}
