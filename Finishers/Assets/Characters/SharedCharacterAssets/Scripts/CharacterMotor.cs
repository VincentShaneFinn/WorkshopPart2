using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{

    [RequireComponent(typeof(Animator))]
    public abstract class CharacterMotor : MonoBehaviour
    {

        #region Class Variables

        [HideInInspector] public bool Strafing = false; // todo strafing currently doesn't let you do anything that basic locomotion does, and is a work in progress
                                                        // also it is currently getting interupted by attack anims that play since they always pause and resume movement
        [HideInInspector] public Transform CurrentLookTarget { private get; set; }
        [HideInInspector] public bool Running = false;

        public bool isGrounded { get; private set; }
        public float turnAmount { get; private set; }
        public float forwardAmount { get; private set; }
 
        public bool Dying {
            get { return Animator.GetBool(AnimContstants.Parameters.DYING_BOOL); }
            set { if (!Dying) Animator.SetBool(AnimContstants.Parameters.DYING_BOOL, value); }
        }
        public virtual bool CanMove
        {
            get { return canMove; }
            set { canMove = value; }
        }
        public virtual bool CanRotate
        {
            get { return canRotate; }
            set { canRotate = value; }
        }

        private bool canMove = true;
        private bool canRotate = true;
        private float origGroundCheckDistance;
        private Vector3 groundNormal;

        #endregion

        #region Character Motor Variables

        [Header("Character Controller Settings")]
        [SerializeField] private float movingTurnSpeed = 1000;
        [SerializeField] private float stationaryTurnSpeed = 1000;
        [Range(1f, 4f)] [SerializeField] private float gravityMultiplier = 2f;
        [Tooltip("May need to modify to get things like  walking to look right with custom models?")]
        [SerializeField] protected float runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [Tooltip("Used to move faster, using rigibody velocity")]
        [SerializeField] protected float moveSpeedMultiplier = 1f;
        [Tooltip("Used to move faster, using rigibody velocity")]
        [SerializeField] protected float runMoveSpeedMultiplier = 1f;
        [Tooltip("Distance from the ground that we consider ourselves grounded")]
        [SerializeField] protected float groundCheckDistance = 0.3f;
        #endregion

        #region Character Animator Variables

        [Header("Animation Settings")]
        [Tooltip("Used to move faster, using animation speed")]
        [SerializeField] protected float animSpeedMultiplier = 1f;
        [Tooltip("Used to move faster, using animation speed")]
        [SerializeField] protected float runAnimSpeedMultiplier = 1.6f;
        [SerializeField] private AnimatorOverrideController AnimatorOverrideControllerConfig;
        protected AnimatorOverrideController animOverrideController;

        public CharAnimStateHandler stateHandler { get; private set; }

        #endregion

        #region Constants

        protected const float HALF = 0.5f;

        #endregion

        #region Components 

        [HideInInspector] public Animator Animator;
        protected Rigidbody rigidBody;
        protected CapsuleCollider capsule;

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
            initialization();
            componentBuilder();
        }

        #region Initialization and ComponentBuilder

        void initialization()
        {
            isGrounded = true;
            turnAmount = 0;
            forwardAmount = 0;
        }

        protected void componentBuilder()
        {
            //Get Components
            Animator = gameObject.GetComponent<Animator>();

            //Setup a new AnimOverrideController

            animOverrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
            Animator.runtimeAnimatorController = animOverrideController;

            //fill new override with data
            var dataOverride = AnimatorOverrideControllerConfig;

            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(dataOverride.overridesCount);
            dataOverride.GetOverrides(overrides);

            //Apply it
            animOverrideController.ApplyOverrides(overrides);


            //Add Components
            stateHandler = gameObject.AddComponent<CharAnimStateHandler>();

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

            //Class Init
            origGroundCheckDistance = groundCheckDistance;
        }

        #endregion

        #region Public Turn Speed Multiplier Getters and Setters
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

        #region Public Move Speed Multiplier Getters and Setters

        // todo build and test functionality for this

        #endregion

        #region Public Character Mover

        // make sure you at least call movecharacter every update or fixed update to update animator parameters
        public void MoveCharacter(Vector3 moveDirection, bool running = false)
        {
            if (Dying)
            {
                disableAllControl();
                return;
            }

            if (running)
            {
                Running = running;
            }

            moveDirection = AdjustMoveDirection(moveDirection);
            turnAmount = Mathf.Atan2(moveDirection.x, moveDirection.z);
            forwardAmount = moveDirection.z;

            if (!CanMove) {
                moveDirection = Vector3.zero;
                forwardAmount = 0;
            }
            if (!CanRotate)
            {
                turnAmount = 0;
            }

            if (Strafing)
            {
                turnAmount = Mathf.Atan2(moveDirection.x, Mathf.Abs(moveDirection.z));
                turnAmount = Mathf.Clamp(turnAmount, -1, 1);
            }
            else
            {
                applyExtraTurnRotation();
            }

            restrictYVelocity();

            // control and velocity handling is different when grounded and airborne:
            if (isGrounded)
            {
                if (Mathf.Abs(rigidBody.velocity.x) + Mathf.Abs(rigidBody.velocity.z) > 5)
                    snapToGround();
            }
            else
            {
                adjustVariablesWhileAirborne();
            }

            // send input and other state parameters to the animator
            updateAnimator(moveDirection);
        }

        private void disableAllControl()
        {
            forwardAmount = 0;
            turnAmount = 0;
            rigidBody.velocity = new Vector3(0,rigidBody.velocity.y, 0);
            updateAnimator(Vector3.zero);
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
            checkGroundStatus();
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal);
            return moveDirection;
        }

        void restrictYVelocity()
        {
            float newYVelocity = rigidBody.velocity.y;
            if (rigidBody.velocity.y > 0)
                newYVelocity = 0;

            rigidBody.velocity = new Vector3(rigidBody.velocity.x, newYVelocity, rigidBody.velocity.z);
        }

        #endregion

        protected abstract void updateAnimator(Vector3 moveDirection);

        #region Other Helper Methods

        // help the character turn faster (this is in addition to root rotation in the animation)
        // modified by the turnSpeedMultiplier
        void applyExtraTurnRotation()
        {
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed * turnSpeedMultiplier, movingTurnSpeed * turnSpeedMultiplier, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        // overriden by player controller since enemies will be snapped to the ground via the NavMeshAgent
        protected virtual void snapToGround()
        {
            return;
        }

        private void adjustVariablesWhileAirborne()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
			rigidBody.AddForce(extraGravityForce);

			groundCheckDistance = rigidBody.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
		}

		private void checkGroundStatus()
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
				Animator.applyRootMotion = true;
			}
			else
			{
				isGrounded = false;
				groundNormal = Vector3.up;
				Animator.applyRootMotion = false;
			}
		}

        #endregion
    }
}
