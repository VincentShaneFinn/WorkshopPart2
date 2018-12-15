using System;
using System.Collections;
using UnityEngine;

namespace Finisher.Characters
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class HumanCharacterController : MonoBehaviour
	{

        #region Class variables, right now mostly deals with movement, jump, and crouch
        public float turnSpeedMultiplier = 1f; // protected force to stay between 0-5

        [SerializeField] float movingTurnSpeed = 360;
		[SerializeField] float stationaryTurnSpeed = 180;
		[SerializeField] float jumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float gravityMultiplier = 2f;
		[SerializeField] float runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float moveSpeedMultiplier = 1f;
		[SerializeField] float animSpeedMultiplier = 1f;
		[SerializeField] float groundCheckDistance = 0.1f;

        Rigidbody myRigidbody;
		protected Animator animator;
		private bool isGrounded; public bool GetIsGrounded() { return isGrounded; }
        float origGroundCheckDistance;
		const float half = 0.5f;
		float turnAmount;
		float forwardAmount;
		Vector3 groundNormal;
		float capsuleHeight;
		Vector3 capsuleCenter;
		CapsuleCollider capsule;

        bool RecentlyJumped = false;
        #endregion

        void Start()
        {
            Initialization();
        }

        protected void Initialization()
        {
            animator = GetComponent<Animator>();
            myRigidbody = GetComponent<Rigidbody>();
            capsule = GetComponent<CapsuleCollider>();
            capsuleHeight = capsule.height;
            capsuleCenter = capsule.center;

            myRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            origGroundCheckDistance = groundCheckDistance;
        }

        //Run testing, would need to add a sprint animation if the speed gets up to running
        // TODO put this back up top and possibly refactor animator to have a run animation
        [SerializeField] float runMoveSpeedMultiplier = 1.5f;
        [SerializeField] float runAnimSpeedMultiplier = 1.5f;
        bool isRunning;

        protected virtual bool CanMove()
        {
            return true;
        }

        public void Move(Vector3 move, bool jump, bool running = false)
		{
            if (!CanMove()) { return; }

            isRunning = running;

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, groundNormal);
			turnAmount = Mathf.Atan2(move.x, move.z);
			forwardAmount = move.z;

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
			UpdateAnimator(move);
		}

        void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
			animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
			animator.SetBool("OnGround", isGrounded);
			if (!isGrounded)
			{
				animator.SetFloat("Jump", myRigidbody.velocity.y);
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
			myRigidbody.AddForce(extraGravityForce);

			groundCheckDistance = myRigidbody.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
		}

		void AttemptToJump(bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// jump!
				myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, jumpPower, myRigidbody.velocity.z);
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
            float newYVelocity = myRigidbody.velocity.y;
            if (myRigidbody.velocity.y > 0)
                newYVelocity = 0;

            myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, newYVelocity, myRigidbody.velocity.z);
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
            {
                transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
            }
        }
        
        // help the character turn faster (this is in addition to root rotation in the animation)
        void ApplyExtraTurnRotation()
		{
            //safety net for turn speed multipliers
            if (turnSpeedMultiplier > 5)
            {
                turnSpeedMultiplier = 5;
            }
            else if (turnSpeedMultiplier < 0)
            {
                turnSpeedMultiplier = 0;
            }
			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed * turnSpeedMultiplier, movingTurnSpeed * turnSpeedMultiplier, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
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
                if (myRigidbody.velocity.y > 0) // protect from going to fast up hill
                {
                    v.y = 0;
                }
                else
                {
                    v.y = myRigidbody.velocity.y;
                }
				myRigidbody.velocity = v;
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Airborne") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
                {
                    SnapToGround();
                }
			}
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
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

        // Notice: due to how we are locking the player to ground when they are walking normal and not jumping, we may have to deal with that hear to let us set a low Y force
        // also adding x and y forces are wierd so do more testing
        void ApplyForce(Vector3 force)
        {
            //ApplyForce
        }
    }
}
