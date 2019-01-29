using UnityEngine;

namespace Finisher.Characters
{

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterState))]
    public abstract class CharacterMotor : MonoBehaviour
    {

        #region Class Variables and public State Variables

        [HideInInspector] public bool Strafing = false;
        [HideInInspector] public Transform CurrentLookTarget;

        public bool Running { get; protected set; }
        public bool isGrounded { get; private set; }
        public float turnAmount { get; private set; }
        public float forwardAmount { get; private set; }

        private bool canMove = true;
        public virtual bool CanMove
        {
            get { return canMove; }
            set { canMove = value; }
        }
        private bool canRotate = true;
        public virtual bool CanRotate
        {
            get { return canRotate; }
            set { canRotate = value; }
        }

        private float origGroundCheckDistance;
        private Vector3 groundNormal;

        #endregion]

        #region Character Motor Variables

        [Header("Character Controller Settings")]
        [SerializeField] private float movingTurnSpeed = 1000;
        [SerializeField] private float stationaryTurnSpeed = 1000;
        [Range(1f, 4f)] [SerializeField] private float gravityMultiplier = 2f;
        [Tooltip("May need to modify to get things like  walking to look right with custom models?")]
        [SerializeField] protected float runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [Tooltip("Used to move faster, using rigibody velocity")]
        [SerializeField] protected float initialMoveSpeedMultiplier = 1f;
        [Tooltip("Used to move faster, using rigibody velocity")]
        [SerializeField] protected float runMoveSpeedMultiplier = 1f;
        [Tooltip("Distance from the ground that we consider ourselves grounded")]
        [SerializeField] protected float groundCheckDistance = 0.3f;

        private float turnSpeedMultiplier = 1f; // protected to stay between 0-5
        private float moveSpeedMultiplier = 1f;

        #endregion

        #region Constants

        protected const float HALF = 0.5f;

        #endregion

        #region Components 

        [HideInInspector] protected Animator animator;
        protected Rigidbody rigidBody;
        protected CapsuleCollider capsule;

        [Header("Rigidbody Component Fields")]
        [SerializeField] CollisionDetectionMode collisionDetectionMode;
        PhysicMaterial frictionlessMaterial;

        [Header("Capsule Collider Component Fields")]
        [SerializeField] Vector3 capsuleColliderCenter = new Vector3(0,1f,0);
        [SerializeField] float capsuleColliderHeight = 2f;
        [SerializeField] float capsuleColliderRadius = 0.5f;

        protected CharacterState characterState;

        #endregion

        protected virtual void Awake()
        {
            initialization();
            componentBuilder();
        }

        protected virtual void Start()
        {
            subscribeToDelegates();
        }

        private void subscribeToDelegates()
        {
            characterState = GetComponent<CharacterState>();
            characterState.DyingState.SubscribeToDeathEvent(KillCharacter);
        }

        void OnDestroy()
        {
            characterState.DyingState.UnsubscribeToDeathEvent(KillCharacter);
        }

        #region Initialization and ComponentBuilder

        private void initialization()
        {
            isGrounded = true;
            turnAmount = 0;
            forwardAmount = 0;
            Running = false;
        }

        private void componentBuilder()
        {
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

        public float MovementSpeedMultiplier
        {
            get { return initialMoveSpeedMultiplier * moveSpeedMultiplier; }
            set
            {
                if (value < 0) { moveSpeedMultiplier = 0; }
                else if (value > 5) { moveSpeedMultiplier = 5; }
                else { moveSpeedMultiplier = value; }
            }
        }

        public void RestoreMovementSpeedMultiplier()
        {
            MovementSpeedMultiplier = 1f;
        }

        #endregion

        #region Public Character Mover

        // make sure you at least call movecharacter every update or fixed update to update animator parameters
        public void MoveCharacter(Vector3 moveDirection, bool running = false)
        {
            if (characterState.DyingState.Dying)
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
				animator.applyRootMotion = true;
			}
			else
			{
				isGrounded = false;
				groundNormal = Vector3.up;
				animator.applyRootMotion = false;
			}
		}

        #endregion

        #region Delegated Methods

        private void KillCharacter()
        {
            capsule.enabled = false;
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        }

        #endregion
    }
}
