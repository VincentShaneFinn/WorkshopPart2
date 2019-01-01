using UnityEngine;

using Finisher.Core;

namespace Finisher.Characters
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof (PlayerCharacterController))]
    public class PlayerInputProcessor : MonoBehaviour
    {
        #region member variables

        public PlayerCharacterController character { get; private set; } // A reference to the ThirdPersonCharacter on the object
        private CombatSystem combatSystem;
        private Transform camRig = null;                  // A reference to the main camera in the scenes transform
        private Vector3 camForward;             // The current forward direction of the camera
        public Vector3 InputMoveDirection { get; private set; }          // the world-relative desired move direction, calculated from the camForward and user input.

        #endregion

        #region Public Interface

        public bool HasMoveInput()
        {
            if (InputMoveDirection != Vector3.zero)
            {
                return true;
            }
            return false;
        }

        #endregion

        private void Start()
        {
            // get the third person character ( this should never be null due to require component )
            character = GetComponent<PlayerCharacterController>();
            combatSystem = GetComponent<CombatSystem>();

            // get the transform of the main camera
            camRig = character.GetMainCameraTransform();
            startFixedDeltaTime = Time.fixedDeltaTime;
        }

        private void Update()
        {
            if (GameManager.instance.GamePaused) { return; }

            if (character.isGrounded)
            {
                processCombatInput();
            }

            testingInputZone();
        }

        private void processCombatInput()
        {
            processAttackInput();
            processDodgeInput();
        }

        private void processAttackInput()
        {
            if (Input.GetButtonDown(InputNames.LightAttack))
            {
                combatSystem.LightAttack();
            }
            if (ControlMethodDetector.GetCurrentControlType() == ControlType.Xbox)
            {
                if (Input.GetAxisRaw(InputNames.HeavyAttack) > 0) // xbox triggers are not buttons
                {
                    combatSystem.HeavyAttack();
                }
            }
            else
            {
                if (Input.GetButtonDown(InputNames.HeavyAttack))
                {
                    combatSystem.HeavyAttack();
                }
            }
        }

        private void processDodgeInput()
        {
            if (Input.GetButtonDown(InputNames.Dodge) || Input.GetKeyDown(KeyCode.Mouse3))
            {
                var dodgeDirection = GetMoveDirection();
                combatSystem.Dodge(dodgeDirection);
            }
        }

        private MoveDirection GetMoveDirection()
        {
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(vertical) >= Mathf.Abs(horizontal)) // forward backward carries more weight
            {
                if(vertical >= 0)
                {
                    return Characters.MoveDirection.Forward;
                }
                else
                {
                    return Characters.MoveDirection.Backward;
                }
            }
            else
            {
                if (horizontal >= 0)
                {
                    return Characters.MoveDirection.Right;
                }
                else
                {
                    return Characters.MoveDirection.Left;
                }
            }
        }

        private float startFixedDeltaTime;
        private void testingInputZone()
        {

            // todo remove this testing code
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
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Time.timeScale = .1f;
                Time.fixedDeltaTime = startFixedDeltaTime * Time.timeScale;
            }
            if (Input.GetKeyUp(KeyCode.Z))
            {
                Time.timeScale = 1;
                Time.fixedDeltaTime = startFixedDeltaTime;
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                character.Strafing = !character.Strafing;
            }
        }

        #region Fixed Update and movement Processing

        void FixedUpdate()
        {
            if (character.CanMove || character.CanRotate)
            {
                processMovementInput();
            }
            else
            {
                character.MoveCharacter(Vector3.zero);
            }
        }

        private void processMovementInput()
        {
            // read inputs
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            SetMoveDirection(horizontal, vertical);

            bool running = Input.GetButton(InputNames.Sprint);

            // pass all parameters to the character control script
            character.MoveCharacter(InputMoveDirection, running);//change to use run button
        }

        private void SetMoveDirection(float horizontal, float vertical)
        {
            // calculate move direction to pass to character
            if (camRig != null)
            {
                // calculate camera relative direction to move:
                camForward = Vector3.Scale(camRig.forward, new Vector3(1, 0, 1)).normalized;
                InputMoveDirection = vertical * camForward + horizontal * camRig.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                InputMoveDirection = vertical * Vector3.forward + horizontal * Vector3.right;
            }
        }

        #endregion

    }
}
