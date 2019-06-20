using UnityEngine;

namespace Finisher.Characters.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerCharacterController))]
    public class PlayerMoveInputProcessor : MonoBehaviour
    {
        #region Member variables

        protected CharacterState characterState;

        // A reference to the main camera in the scenes transform
        private Vector3 camForward;

        public PlayerCharacterController character { get; private set; } // A reference to the ThirdPersonCharacter on the object

        // The current forward direction of the camera
        public Vector3 InputMoveDirection { get; private set; }          // the world-relative desired move direction, calculated from the camForward and user input.

        #endregion Member variables

        private void Start()
        {
            // get the third person character ( this should never be null due to require component )
            character = GetComponent<PlayerCharacterController>();
            characterState = GetComponent<CharacterState>();
        }

        #region Fixed Update and movement Processing

        private void FixedUpdate()
        {
            if (character.CanMove || character.CanRotate)
            {
                processMovementInput();
                processLookInput();
            }
            else
            {
                if (characterState.Dodging)
                {
                    character.transform.rotation = Quaternion.identity;
                }
                character.MoveCharacter(Vector3.zero);
            }
        }

        private void processMovementInput()
        {
            // read inputs
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            SetMoveDirection(horizontal, vertical);

            // pass all parameters to the character control script
            character.MoveCharacter(InputMoveDirection, FinisherInput.isSpriting());
        }

        private void processLookInput()
        {
            Vector3 playerDirection = Vector3.right * Input.GetAxisRaw("Mouse X") + Vector3.forward * Input.GetAxisRaw("Mouse Y");
            if (playerDirection.sqrMagnitude > 0.0f)
            {
                character.transform.rotation = Quaternion.LookRotation(playerDirection, Vector3.up);
            }
        }

        private void SetMoveDirection(float horizontal, float vertical)
        {
            InputMoveDirection = vertical * Vector3.forward + horizontal * Vector3.right;
        }

        #endregion Fixed Update and movement Processing

        #region Public Interface

        public bool HasMoveInput()
        {
            if (InputMoveDirection != Vector3.zero)
            {
                return true;
            }
            return false;
        }

        #endregion Public Interface
    }
}