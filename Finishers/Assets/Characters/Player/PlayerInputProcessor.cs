using System.Linq;
using UnityEngine;

using Finisher.Cameras;
using Finisher.Core;

namespace Finisher.Characters
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof (PlayerCharacterController))]
    public class PlayerInputProcessor : MonoBehaviour
    {
        #region member variables

        [SerializeField] float mainRange = 3f;
        [SerializeField] float extraRange = 1.5f;

        private PlayerCharacterController character = null; // A reference to the ThirdPersonCharacter on the object
        private CombatSystem combatSystem;
        private Transform camRig = null;                  // A reference to the main camera in the scenes transform
        private Vector3 camForward;             // The current forward direction of the camera
        private Vector3 moveDirection;          // the world-relative desired move direction, calculated from the camForward and user input.

        private Transform combatTarget;

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

            combatTarget = GetCombatTarget();
            if (combatTarget)
            {
                character.UseStraffingTarget = true;
                character.CurrentLookTarget = combatTarget;
            }
            else
            {
                character.UseStraffingTarget = false;
            }

            if (character.isGrounded)
            {
                processCombatInput();
            }

            testingInputZone();
        }

        private void processCombatInput()
        {
            if (Input.GetButtonDown(InputNames.LightAttack))
            {
                combatSystem.LightAttack();
            }
            if(ControlMethodDetector.GetCurrentControlType() == ControlType.Xbox)
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


            if (Input.GetButtonDown(InputNames.Dodge) || Input.GetKeyDown(KeyCode.Mouse3))
            {
                var moveDirection = GetMoveDirection();
                combatSystem.Dodge(moveDirection);
            }
        }

        // todo, make sure it gets the direction with respect to the camera rig rotation
        // and that it makes you dodge the way you expect
        MoveDirection GetMoveDirection()
        {
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(vertical) >= Mathf.Abs(horizontal)) // forward backward carries more weight
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
                if (horizontal >= 0)
                {
                    return MoveDirection.Right;
                }
                else
                {
                    return MoveDirection.Left;
                }
            }
        }

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
            }
            if (Input.GetKeyUp(KeyCode.Z))
            {
                Time.timeScale = 1;
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                character.Strafing = !character.Strafing;
            }
        }

        private Transform GetCombatTarget()
        {
            //Get nearby enemy colliders
            int layerMask = 1 << LayerNames.EnemyLayer;
            var enemyColliders = Physics.OverlapSphere(transform.position, mainRange, layerMask).ToList();
            if (enemyColliders.Count <= 0) { return null; }

            enemyColliders = enemyColliders.OrderBy(
                enemy => Vector2.Distance(this.transform.position, enemy.transform.position)
                ).ToList();

            Transform alternateTarget = null;
            foreach (Collider enemyCollider in enemyColliders)
            {
                Transform target = enemyCollider.transform;
                var targetMotor = target.gameObject.GetComponent<CharacterMotor>();
                if (targetMotor == null) { continue; }
                else if (targetMotor.Dying) { continue; }

                // get the current angle of that enemy to the left or right of you
                Vector3 targetDir = target.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);

                // check if the enemy is in your field of vision
                float mainFOV = 90f;
                float extraFOV = 190f;
                if (angle < mainFOV / 2)
                {
                    //Debug.DrawLine(transform.position + Vector3.up, target.position + Vector3.up, Color.red);
                    return target;
                }
                else if (Vector3.Distance(transform.position, target.position) <= extraRange && angle < extraFOV / 2)
                {
                    if (alternateTarget == null)
                    {
                        alternateTarget = target;
                    }
                }
            }
            //if(alternateTarget)
                //Debug.DrawLine(transform.position + Vector3.up, alternateTarget.transform.position + Vector3.up, Color.white);
            return alternateTarget;
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

            // calculate move direction to pass to character
            if (camRig != null)
            {
                // calculate camera relative direction to move:
                camForward = Vector3.Scale(camRig.forward, new Vector3(1, 0, 1)).normalized;
                moveDirection = vertical * camForward + horizontal * camRig.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                moveDirection = vertical * Vector3.forward + horizontal * Vector3.right;
            }

            //use to be how walking was done, running may need a small rework
            //if (Input.GetKey(KeyCode.LeftShift)) moveDirection *= 0.5f;

            // pass all parameters to the character control script
            character.MoveCharacter(moveDirection, Input.GetKey(KeyCode.LeftShift));//change to use run button
        }

        #endregion

    }
}
