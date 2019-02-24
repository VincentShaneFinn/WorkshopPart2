using UnityEngine;

using Finisher.Characters;
using Finisher.Characters.Player; //TODO: Close but not yet fully independant from our player
using Finisher.Core;

namespace Finisher.Cameras
{
    public class CameraLookController : PivotBasedCameraRig
    {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        #region Class Variables

        public Transform OptionalAutoLookTarget = null; // set what the autocamera will look at
        public bool ForceAutoLook = false; // force it to use the auto camera
        public Transform NewFollowTarget = null; // used to switch follow target to enemy during grab
        private Transform combatTarget;

        [SerializeField] private float moveSpeed = 10f;
        [Range(0f, 10f)] [SerializeField] private float turnSpeed = 1.5f;   // How fast the rig will rotate from user input.
        [SerializeField] float turnSmoothing = 0.0f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
        [SerializeField] float tiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
        [SerializeField] float tiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.
        [SerializeField] bool verticalAutoReturn = false;           // set wether or not the vertical axis should auto return

        //AutoCam variables
        [SerializeField] float autoCamTurnSpeed = 1.5f; // TODO allow user to change both turn speed and autoCam Turn speed
        [SerializeField] float timeUntilAutoCam = 1f;
        [SerializeField] float targetVelocityLowerLimit = 4f;// the minimum velocity above which the camera turns towards the object's velocity. Below this we use the object's forward direction.
        [SerializeField] float smoothTurnFactor = 0.2f; // the smoothing for the camera's rotation
        private float countUntilAutoCam = 0f;
        private float currentTurnAmount; // How much to turn the camera
        private float turnSpeedVelocityChange; // The change in the turn speed velocity

        private PlayerCharacterController character;
        [SerializeField] private CharacterStateSO playerState;
        private EnemyManager enemyManager;

        private float lookAngle;                    // The rig's y axis rotation.
        private float tiltAngle;                    // The pivot's x axis rotation.
        private const float lookDistance = 100f;    // How far in front of the pivot the character's look target is.
		private Vector3 pivotEulers;
		private Quaternion pivotTargetRot;
		private Quaternion transformTargetRot;
        private bool usingAutoCam = false;
        float inputX;
        float inputY;
        [SerializeField] float rotationDisabledAngle = 20f;
        [SerializeField] float rotationReEnableAngle = 3f;
        bool rotateUntilClose = false;
        #endregion

        protected override void Awake()
        {
            base.Awake();

            //Initialize variables
			pivotEulers = pivot.rotation.eulerAngles;
	        pivotTargetRot = pivot.transform.localRotation;
			transformTargetRot = transform.localRotation;
            character = FindObjectOfType<PlayerCharacterController>();
            enemyManager = FindObjectOfType<EnemyManager>();
        }

        protected void Update()
        {
            
            if (GameManager.instance.GamePaused)
            {
                return;
            }

            // Read the user input
            inputX = Input.GetAxis("Mouse X");
            inputY = Input.GetAxis("Mouse Y");
        
            SetUsingAutoCam();
            if (playerState.IsGrabbing)
            {
                usingAutoCam = false;
            }
            if (!usingAutoCam)
            {
                HandleRotationMovement();
            }
            if (usingAutoCam)
            {
                AutoRotateCamera(Time.deltaTime);
            }
        }

        #region Helper Methods

        // check if we should start auto rotating the camera if no input for time [timeUntilAutoCam]
        private void SetUsingAutoCam()
        {
            if (ForceAutoLook) //use auto cam if forceautolook is true
            {
                ChangeCameraMode(true);
                return;
            }

            SetCameraMode();
        }

        private void SetCameraMode()
        {
            if (Mathf.Abs(inputX) > 0 || Mathf.Abs(inputY) > 0) // user input found
            {
                countUntilAutoCam = 0f;
            }
            else
            {
                if (countUntilAutoCam >= timeUntilAutoCam) // start using autocam
                {
                    float angleToTarget = 360;
                    if (playerState.CombatTarget != null)
                    {
                        Vector3 relativePos = playerState.CombatTarget.transform.position - transform.position;

                        // the second argument, upwards, defaults to Vector3.up
                        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                        angleToTarget = Quaternion.Angle(transform.rotation, rotation);
                    }
                    if (!(angleToTarget < rotationDisabledAngle && angleToTarget > rotationDisabledAngle * -1) ||
                    (rotateUntilClose && !(angleToTarget < rotationReEnableAngle && angleToTarget > rotationReEnableAngle * -1)))
                    {
                        ChangeCameraMode(true);
                        rotateUntilClose = true;
                        return;
                    }
                }
            }

            countUntilAutoCam += Time.deltaTime;

            ChangeCameraMode(false);
            rotateUntilClose = false;
        }

        private void ChangeCameraMode(bool switchToAutoCam)
        {
            if (usingAutoCam == switchToAutoCam)
                return;
            
            usingAutoCam = switchToAutoCam;
            if (!usingAutoCam)
            {
                lookAngle = transform.eulerAngles.y;
                tiltAngle = pivot.transform.localEulerAngles.x;
                if (tiltAngle > 180) {
                    tiltAngle -= 360;
                }
            }

        }

        #endregion

        // handle player input to look around
        private void HandleRotationMovement()
        {
            // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
            lookAngle += inputX*turnSpeed;

            // Rotate the rig (the root object) around Y axis only:
            transformTargetRot = Quaternion.Euler(0f, lookAngle, 0f);

            if (verticalAutoReturn)
            {
                // For tilt input, we need to behave differently depending on whether we're using mouse or touch input:
                // on mobile, vertical input is directly mapped to tilt value, so it springs back automatically when the look input is released
                // we have to test whether above or below zero because we want to auto-return to zero even if min and max are not symmetrical.
                tiltAngle = inputY > 0 ? Mathf.Lerp(0, -tiltMin, inputY) : Mathf.Lerp(0, tiltMax, -inputY);
            }
            else
            {
                // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
                tiltAngle -= inputY * turnSpeed;
                // and make sure the new value is within the tilt range
                tiltAngle = Mathf.Clamp(tiltAngle, -tiltMin, tiltMax);
            }

            // Tilt input around X is applied to the pivot (the child of this object)
			pivotTargetRot = Quaternion.Euler(tiltAngle, pivotEulers.y , pivotEulers.z);

			if (turnSmoothing > 0)
			{
				pivot.localRotation = Quaternion.Slerp(pivot.localRotation, pivotTargetRot, turnSmoothing * Time.deltaTime);
				transform.localRotation = Quaternion.Slerp(transform.localRotation, transformTargetRot, turnSmoothing * Time.deltaTime);
			}
			else
			{
				pivot.localRotation = pivotTargetRot;
				transform.localRotation = transformTargetRot;
			}
        }

        #region Follow Target and AutoRotate

        // Moves the camera rig to follow the target over some speed
        protected override void FollowTarget(float deltaTime)
        {
            if (!(deltaTime > 0) || followTarget == null) return;

            Transform targetToUse;

            if (NewFollowTarget)
            {
                targetToUse = NewFollowTarget;
            }
            else
            {
                targetToUse = followTarget;
            }

            // Move the rig towards target position.
            transform.position = Vector3.Lerp(transform.position, targetToUse.position, deltaTime * moveSpeed);
        }

        // automatically rotate camera to face player if no input for some time [timeUntilAutoCam]
        private void AutoRotateCamera(float deltaTime)
        {

            if (!followTarget)
            {
                return;
            }

            if (character.CombatTarget)
            {
                combatTarget = character.CombatTarget;
            }
            if ((combatTarget && combatTarget.GetComponent<CharacterState>().Dying) ||
                !enemyManager.InCombat)
            {
                combatTarget = null;
            }

            // initialise some vars, we'll be modifying these in a moment
            var targetForward = followTarget.forward;
            var targetUp = followTarget.up;

            // in follow velocity mode, the camera's rotation is aligned towards the object's velocity direction
            // but only if the object is traveling faster than a given threshold.

            if (targetRigidbody.velocity.magnitude > targetVelocityLowerLimit)
            {
                // velocity is high enough, so we'll use the target's velocty
                targetForward = targetRigidbody.velocity.normalized;
                targetUp = Vector3.up;
            }
            else
            {
                targetUp = Vector3.up;
            }

            currentTurnAmount = Mathf.SmoothDamp(currentTurnAmount, 1, ref turnSpeedVelocityChange, smoothTurnFactor);

            // camera's rotation is split into two parts, which can have independend speed settings:
            // rotating towards the target's forward direction (which encompasses its 'yaw' and 'pitch')

            targetForward.y = 0;
            if (targetForward.sqrMagnitude < float.Epsilon)
            {
                targetForward = transform.forward;
            }

            Quaternion desiredLookRotation;
            Quaternion desiredTiltRotation = Quaternion.identity;

            //If there is an active optional look target, look at that
            if (OptionalAutoLookTarget != null && OptionalAutoLookTarget.gameObject.activeSelf)
            {
                Quaternion rotationToTarget = Quaternion.LookRotation(OptionalAutoLookTarget.transform.position - transform.position);
                desiredLookRotation = new Quaternion(0, rotationToTarget.y, 0, rotationToTarget.w);
                desiredTiltRotation = new Quaternion(rotationToTarget.x, 0, 0, rotationToTarget.w);
            }
            else if (combatTarget != null)
            {
                Quaternion rotationToTarget = Quaternion.LookRotation(combatTarget.transform.position - transform.position);
                desiredLookRotation = new Quaternion(0, rotationToTarget.y, 0, rotationToTarget.w);
                desiredTiltRotation = new Quaternion(rotationToTarget.x, 0, 0, rotationToTarget.w);
            }
            else if (targetForward != Vector3.zero)
            {
                desiredLookRotation = Quaternion.LookRotation(targetForward, Vector3.up);
            }
            else
            {
                desiredLookRotation = transform.rotation;
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, desiredLookRotation, autoCamTurnSpeed * currentTurnAmount * deltaTime);
            pivot.transform.localRotation = Quaternion.Lerp(pivot.transform.localRotation, desiredTiltRotation, autoCamTurnSpeed * currentTurnAmount * deltaTime); ;
        }

        #endregion


        void OnDrawGizmos()
        {
            float MAINRANGE = 5f;
            float MAINFOV = 30f;

            Gizmos.color = Color.green;

            // local coordinate rotation around the Y axis to the given angle
            Quaternion rotation = Quaternion.AngleAxis(MAINFOV, Vector3.up);
            // add the desired distance to the direction
            Vector3 addDistanceToDirection = rotation * transform.forward * MAINRANGE;
            Vector3 destination = transform.position + addDistanceToDirection;

            Quaternion rotation2 = Quaternion.AngleAxis(-MAINFOV, Vector3.up);
            // add the desired distance to the direction
            Vector3 addDistanceToDirection2 = rotation2 * transform.forward * MAINRANGE;
            Vector3 destination2 = transform.position + addDistanceToDirection2;

            Gizmos.DrawLine(transform.position + Vector3.up, destination + Vector3.up);
            Gizmos.DrawLine(transform.position + Vector3.up, destination2 + Vector3.up);
        }
    }
}
