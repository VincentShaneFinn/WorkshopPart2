using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Finisher.Cameras
{

    public class FreeLookCam : PivotBasedCameraRig
    {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        #region Class Variables
        [SerializeField] float m_MoveSpeed = 1f;                      // How fast the rig will move to keep up with the target's position.
        [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.
        [SerializeField] float m_TurnSmoothing = 0.0f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
        [SerializeField] float m_TiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
        [SerializeField] float m_TiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.
        [SerializeField] bool m_LockCursor = false;                   // Whether the cursor should be hidden and locked.
        [SerializeField] bool m_VerticalAutoReturn = false;           // set wether or not the vertical axis should auto return

        //AutoCam variables
        [SerializeField] float autoCamTurnSpeed = 1.5f; // TODO allow user to change both turn speed and autoCam Turn speed
        [SerializeField] float timeUntilAutoCam = 1f;
        [SerializeField] float m_TargetVelocityLowerLimit = 4f;// the minimum velocity above which the camera turns towards the object's velocity. Below this we use the object's forward direction.
        [SerializeField] float m_RollSpeed = 0.2f;// How fast the rig will roll (around Z axis) to match target's roll.
        [SerializeField] float m_SmoothTurnTime = 0.2f; // the smoothing for the camera's rotation
        private float countUntilAutoCam = 0f;
        private float m_CurrentTurnAmount; // How much to turn the camera
        private float m_TurnSpeedVelocityChange; // The change in the turn speed velocity
        private Vector3 m_RollUp = Vector3.up;// The roll of the camera around the z axis ( generally this will always just be up )


        private float m_LookAngle;                    // The rig's y axis rotation.
        private float m_TiltAngle;                    // The pivot's x axis rotation.
        private const float k_LookDistance = 100f;    // How far in front of the pivot the character's look target is.
		private Vector3 m_PivotEulers;
		private Quaternion m_PivotTargetRot;
		private Quaternion m_TransformTargetRot;
        private bool autoCam = false;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            // Lock or unlock the cursor.
            Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None; // TODO move cursor lock to the pause menu handler, remember to delete m_LockCursor and replace with is paused
            Cursor.visible = !m_LockCursor;

            //Initialize variables
			m_PivotEulers = m_Pivot.rotation.eulerAngles;
	        m_PivotTargetRot = m_Pivot.transform.localRotation;
			m_TransformTargetRot = transform.localRotation;
        }


        protected void Update()
        {
            // TODO don't ever allow auto camera for mouse and keyboard
            if (autoCam != UseAutoCam())
            {
                autoCam = UseAutoCam();
                m_LookAngle = transform.eulerAngles.y;
            }

            if (!autoCam)
            {
                HandleRotationMovement();
            }
            
            if (m_LockCursor && Input.GetMouseButtonUp(0))
            {
                Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !m_LockCursor;
            }
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Moves the camera rig to follow the target over some speed
        protected override void FollowTarget(float deltaTime)
        {
            if (!(deltaTime > 0) || m_Target == null) return;

            // Move the rig towards target position.
            transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime * m_MoveSpeed);

            if (autoCam)
            {
                AutoRotateCamera(deltaTime);
            }
        }

        // check if we should start auto rotating the camera if no input for time [timeUntilAutoCam]
        private bool UseAutoCam()
        {
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");

            if (x <= Mathf.Epsilon && y <= Mathf.Epsilon)
            {
                if(countUntilAutoCam >= timeUntilAutoCam)
                    return true;
            }
            else
            {
                countUntilAutoCam = 0f;
            }

            countUntilAutoCam += Time.deltaTime;

            return false;
        }

        // automatically rotate camera to face player if no input for some time [timeUntilAutoCam]
        private void AutoRotateCamera(float deltaTime)
        {


            // initialise some vars, we'll be modifying these in a moment
            var targetForward = m_Target.forward;
            var targetUp = m_Target.up;

            // in follow velocity mode, the camera's rotation is aligned towards the object's velocity direction
            // but only if the object is traveling faster than a given threshold.

            if (targetRigidbody.velocity.magnitude > m_TargetVelocityLowerLimit)
            {
                // velocity is high enough, so we'll use the target's velocty
                targetForward = targetRigidbody.velocity.normalized;
                targetUp = Vector3.up;
            }
            else
            {
                targetUp = Vector3.up;
            }
            m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, 1, ref m_TurnSpeedVelocityChange, m_SmoothTurnTime);

            // camera's rotation is split into two parts, which can have independend speed settings:
            // rotating towards the target's forward direction (which encompasses its 'yaw' and 'pitch')

            targetForward.y = 0;
            if (targetForward.sqrMagnitude < float.Epsilon)
            {
                targetForward = transform.forward;
            }
            var rollRotation = Quaternion.LookRotation(targetForward, m_RollUp);

            // and aligning with the target object's up direction (i.e. its 'roll')
            m_RollUp = m_RollSpeed > 0 ? Vector3.Slerp(m_RollUp, targetUp, m_RollSpeed * deltaTime) : Vector3.up;
            transform.rotation = Quaternion.Lerp(transform.rotation, rollRotation, autoCamTurnSpeed * m_CurrentTurnAmount * deltaTime);
        }

        // handle player input to look around
        private void HandleRotationMovement()
        {
			if(Time.timeScale < float.Epsilon)
			return;

            // Read the user input
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");

            // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
            m_LookAngle += x*m_TurnSpeed;

            // Rotate the rig (the root object) around Y axis only:
            m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);

            if (m_VerticalAutoReturn)
            {
                // For tilt input, we need to behave differently depending on whether we're using mouse or touch input:
                // on mobile, vertical input is directly mapped to tilt value, so it springs back automatically when the look input is released
                // we have to test whether above or below zero because we want to auto-return to zero even if min and max are not symmetrical.
                m_TiltAngle = y > 0 ? Mathf.Lerp(0, -m_TiltMin, y) : Mathf.Lerp(0, m_TiltMax, -y);
            }
            else
            {
                // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
                m_TiltAngle -= y*m_TurnSpeed;
                // and make sure the new value is within the tilt range
                m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);
            }

            // Tilt input around X is applied to the pivot (the child of this object)
			m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y , m_PivotEulers.z);

			if (m_TurnSmoothing > 0)
			{
				m_Pivot.localRotation = Quaternion.Slerp(m_Pivot.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
				transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
			}
			else
			{
				m_Pivot.localRotation = m_PivotTargetRot;
				transform.localRotation = m_TransformTargetRot;
			}
        }
    }
}
