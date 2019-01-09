using UnityEngine;

namespace Finisher.Characters
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof (PlayerCharacterController))]
    public class PlayerMoveInputProcessor : MonoBehaviour
    {
        #region Member variables

        public PlayerCharacterController character { get; private set; } // A reference to the ThirdPersonCharacter on the object
        private Transform camRig = null;                  // A reference to the main camera in the scenes transform
        private Vector3 camForward;             // The current forward direction of the camera
        public Vector3 InputMoveDirection { get; private set; }          // the world-relative desired move direction, calculated from the camForward and user input.

        #endregion

        private void Start()
        {
            // get the third person character ( this should never be null due to require component )
            character = GetComponent<PlayerCharacterController>();

            // get the transform of the main camera
            camRig = character.GetMainCameraTransform();
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

    }
}
