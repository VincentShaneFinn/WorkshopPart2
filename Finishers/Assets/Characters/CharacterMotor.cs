using System;
using System.Collections;
using UnityEngine;

namespace Finisher.Characters
{
	public abstract class CharacterMotor : MonoBehaviour
	{

        #region Character Motor Variables
        [Header("Character Controller Settings")]
        [SerializeField] protected float movingTurnSpeed = 1000;
        [SerializeField] protected float stationaryTurnSpeed = 1000;
        [SerializeField] protected float jumpPower = 7f;
        [Range(1f, 4f)] [SerializeField] protected float gravityMultiplier = 2f;
        [Tooltip("May need to modify to get things like jumping / walking to look right with custom models?")]
        [SerializeField] protected float runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [Tooltip("Used to move faster, using rigibody velocity")]
        [SerializeField] protected float moveSpeedMultiplier = 1f;
        [Tooltip("Used to move faster, using rigibody velocity")]
        [SerializeField] protected float runMoveSpeedMultiplier = 1f;
        [Tooltip("Distance from the ground that we consider ourselves grounded")]
        [SerializeField] protected float groundCheckDistance = 0.3f;
        #endregion

        #region Character Animator Variables
        [Tooltip("Used to move faster, using animation speed")]
        [SerializeField] protected float animSpeedMultiplier = 1f;
        [Tooltip("Used to move faster, using animation speed")]
        [SerializeField] protected float runAnimSpeedMultiplier = 1.6f;
        #endregion

        #region Constants
        protected const string LOCOMOTION_STATE = "Basic Locomotion";
        protected const float half = 0.5f;
        #endregion

        #region General Class variables
        protected bool isGrounded; public bool GetIsGrounded() { return isGrounded; }
        protected float origGroundCheckDistance;
        protected float turnAmount;
        protected float forwardAmount;
        protected Vector3 groundNormal;
        protected bool RecentlyJumped = false;
        protected bool isRunning;
        #endregion

        #region Components 
        protected Animator animator;
        protected Rigidbody rigidBody;
        protected CapsuleCollider capsule;

        [Header("Animator Component Fields")]
        [SerializeField] RuntimeAnimatorController runtimeAnimatorController;
        [SerializeField] Avatar avatar;

        [Header("Rigidbody Component Fields")]
        [SerializeField] CollisionDetectionMode collisionDetectionMode;
        PhysicMaterial frictionlessMaterial;

        [Header("Capsule Collider Component Fields")]
        [SerializeField] Vector3 capsuleColliderCenter = new Vector3(0,1f,0);
        [SerializeField] float capsuleColliderHeight = 2f;
        [SerializeField] float capsuleColliderRadius = 0.5f;

        #endregion

        void Awake()
        {
            ComponentBuilder();
        }

        protected virtual void Start()
        {
            //Class Init
            origGroundCheckDistance = groundCheckDistance;
        }

        #region ComponentBuilder
        protected void ComponentBuilder()
        {
            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = runtimeAnimatorController;
            animator.avatar = avatar;
            rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidBody.useGravity = true;
            rigidBody.isKinematic = false;
            rigidBody.collisionDetectionMode = collisionDetectionMode;
            capsule = gameObject.AddComponent<CapsuleCollider>();
            frictionlessMaterial = new PhysicMaterial();
            frictionlessMaterial.dynamicFriction = 0;
            frictionlessMaterial.staticFriction = 0;
            frictionlessMaterial.bounciness = 0;
            frictionlessMaterial.frictionCombine = PhysicMaterialCombine.Multiply;
            frictionlessMaterial.bounceCombine = PhysicMaterialCombine.Average;
            capsule.material = frictionlessMaterial;
            capsule.center = capsuleColliderCenter;
            capsule.height = capsuleColliderHeight;
            capsule.radius = capsuleColliderRadius;
        }
        #endregion

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

        protected abstract void UpdateAnimator(Vector3 moveDirection);

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

        // help the character turn faster (this is in addition to root rotation in the animation)
        // modified by the turnSpeedMultiplier
        void ApplyExtraTurnRotation()
        {
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed * turnSpeedMultiplier, movingTurnSpeed * turnSpeedMultiplier, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        protected abstract void AttemptToJump(bool jump);

        void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
			rigidBody.AddForce(extraGravityForce);

			groundCheckDistance = rigidBody.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
		}

        protected void RestrictYVelocity()
        {
            float newYVelocity = rigidBody.velocity.y;
            if (rigidBody.velocity.y > 0)
                newYVelocity = 0;

            rigidBody.velocity = new Vector3(rigidBody.velocity.x, newYVelocity, rigidBody.velocity.z);
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
