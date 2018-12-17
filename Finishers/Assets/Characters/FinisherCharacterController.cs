using System;
using System.Collections;
using UnityEngine;

namespace Finisher.Characters
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class FinisherCharacterController : MonoBehaviour
	{

        #region Class variables, right now mostly deals with movement, jump, and crouch

        [Header("Character Controller Settings")]
        [SerializeField] float movingTurnSpeed = 360;
		[SerializeField] float stationaryTurnSpeed = 180;
		[SerializeField] float jumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float gravityMultiplier = 2f;
        [Tooltip("May need to modify to get things like jumping / walking to look right with custom models?")]
		[SerializeField] float runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[Tooltip("Used to move faster, but not modify the speed of the animation")]
        [SerializeField] float moveSpeedMultiplier = 1f;
        [Tooltip("Used to move faster, by making the animation play faster")]
        [SerializeField] float animSpeedMultiplier = 1f;
        [Tooltip("Used to move faster, but not modify the speed of the animation")]
        [SerializeField] float runMoveSpeedMultiplier = 1.5f;
        [Tooltip("Used to move faster, by making the animation play faster")]
        [SerializeField] float runAnimSpeedMultiplier = 1.5f;
        [Tooltip("Distance from the ground that we consider ourselves grounded")]
        [SerializeField] float groundCheckDistance = 0.1f;

        Rigidbody rigidBody;
		protected Animator animator;
		private bool isGrounded; public bool GetIsGrounded() { return isGrounded; }
        float origGroundCheckDistance;
		const float half = 0.5f;
		float turnAmount;
		float forwardAmount;
		Vector3 groundNormal;
		CapsuleCollider capsule;

        bool RecentlyJumped = false;
        bool isRunning;
        #endregion

        void Start()
        {
            Initialization();
        }

        // TODO make the initializer start in awake instead, and add the all the components that we need
        // bonus make an editor script to make this collapsed by default in the inspector so we dont need to see it
        // or just make put in in another class that this class requires
        protected void Initialization()
        {
            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody>();
            capsule = GetComponent<CapsuleCollider>();

            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            origGroundCheckDistance = groundCheckDistance;
        }

        // TODO what is this for, or should we use this to tell if the character can move or not, same with rotation
        // goal is to get the player and ai character using the same method names to do these 4 things
            // stop and start moving
            // stop and start rotating
            // move slower or faster
            // rotate slower or faster
        protected virtual bool CanMove()
        {
            return true;
        }

        // TODO this needs to be called everyframe by what is using it to make sure it is being snaped to the ground during an action
        // see if we can put snap to ground in an update loop that is always run if in a certain state
        // note, maybe this should be a character movement controller, and we can also have a character action controller, and both use each other?
        public void Move(Vector3 moveDirection, bool jump, bool running = false)
        {
            if (!CanMove()) { return; }

            isRunning = running;

            moveDirection = AdjustMoveDirection(moveDirection);
            turnAmount = Mathf.Atan2(moveDirection.x, moveDirection.z);
            forwardAmount = moveDirection.z;

            ApplyExtraTurnRotation();

            // control and velocity handling is different when grounded and airborne:
            if (isGrounded)
            {
                AttemptToJump(jump);
            }
            else
            {
                HandleAirborneMovement();
            }

            // send input and other state parameters to the animator
            UpdateAnimator(moveDirection);
        }

        private Vector3 AdjustMoveDirection(Vector3 moveDirection)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (moveDirection.magnitude > 1f)
            {
                moveDirection.Normalize();
            }
            moveDirection = transform.InverseTransformDirection(moveDirection);
            CheckGroundStatus();
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal);
            return moveDirection;
        }

        void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
			animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
			animator.SetBool("OnGround", isGrounded);
			if (!isGrounded)
			{
				animator.SetFloat("Jump", rigidBody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat(
					animator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);
			float jumpLeg = (runCycle < half ? 1 : -1) * forwardAmount;
			if (isGrounded)
			{
				animator.SetFloat("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded") && move.magnitude > 0)
			{
                if (!isRunning)
                {
                    animator.speed = animSpeedMultiplier;
                }
                else
                    animator.speed = runAnimSpeedMultiplier;
			}
			else
			{
				// don't use that while airborne
				animator.speed = 1;
			}
		}

		void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
			rigidBody.AddForce(extraGravityForce);

			groundCheckDistance = rigidBody.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
		}

		void AttemptToJump(bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// jump!
				rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpPower, rigidBody.velocity.z);
				isGrounded = false;
				animator.applyRootMotion = false;
				groundCheckDistance = 0.1f;

                if (!RecentlyJumped)
                {
                    RecentlyJumped = true;
                    float timeToFreeJump = .3f;
                    StartCoroutine(FreeJumpOverTime(timeToFreeJump));
                }
            }
		}

        IEnumerator FreeJumpOverTime(float time)
        {
            yield return new WaitForSeconds(time);
            RecentlyJumped = false;
        }

        private void SnapToGround()
        {
            float newYVelocity = rigidBody.velocity.y;
            if (rigidBody.velocity.y > 0)
                newYVelocity = 0;

            rigidBody.velocity = new Vector3(rigidBody.velocity.x, newYVelocity, rigidBody.velocity.z);
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
            {
                transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
            }
        }
        
        // help the character turn faster (this is in addition to root rotation in the animation)
        void ApplyExtraTurnRotation()
		{
			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed * turnSpeedMultiplier, movingTurnSpeed * turnSpeedMultiplier, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}

        #region Turn Speed Multiplier Getters and Setters
        float turnSpeedMultiplier = 1f; // protected to stay between 0-5
        public float TurnSpeedMultiplier
        {
            get { return turnSpeedMultiplier; }
            set
            {
                if (value < 0) { turnSpeedMultiplier = 0; }
                else if (value > 5) { turnSpeedMultiplier = 5; }
                else { turnSpeedMultiplier = value; }
            }
        }

        //Turn speed modifiers
        public void SetInstantCharacterRotation()
        {
            turnSpeedMultiplier = 5; // lets say max is the 5
        }
        public void LockCharacterRotation()
        {
            turnSpeedMultiplier = 0;
        }
        public void RestoreRotationLerp()
        {
            turnSpeedMultiplier = 1;
        }
        #endregion

        public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (isGrounded && Time.deltaTime > 0)
			{
                Vector3 v;
                if (isRunning)
                {
                    v = (animator.deltaPosition * runMoveSpeedMultiplier) / Time.deltaTime;
                }
                else
                {
                    v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
                }

                // we preserve the existing y part of the current velocity.
                if (rigidBody.velocity.y > 0) // protect from going to fast up hill
                {
                    v.y = 0;
                }
                else
                {
                    v.y = rigidBody.velocity.y;
                }
				rigidBody.velocity = v;
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Airborne") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
                {
                    SnapToGround();
                }
			}
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;

			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));

			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
			{
				groundNormal = hitInfo.normal;
				isGrounded = true;
				animator.applyRootMotion = true;
			}
			else
			{
				isGrounded = false;
				groundNormal = Vector3.up;
				animator.applyRootMotion = false;
			}
		}
    }
}
