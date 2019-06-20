using Finisher.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Finisher.Characters.Player
{
    public class PlayerCharacterController : CharacterAnimator
    {
        #region Class Variables

        [Header("Player Combat Target Hitbox Settings")]
        [SerializeField]
        private GameObject CombatTargetIndicator;

        //[Header("Player Controller Specific")]
        [SerializeField] private float MAINRANGE = 3f;

        [SerializeField] private float SECONDARY_HITBOX_RANGE = 1.5f;
        [SerializeField] private float MAINFOV = 50f;
        [SerializeField] private float SECONDARY_HITBOX = 100f;
        [SerializeField] private float DIRECTIONAL_INPUT_HITBOX = 35f;
        private bool usingLSInput = false;
        private List<Collider> enemyColliders = null;
        private PlayerMoveInputProcessor playerIP;
        [SerializeField] private CharacterStateSO playerState;
        public Transform CombatTarget { get; private set; }

        #endregion Class Variables

        #region Public Interface

        public Transform GetMainCameraTransform()
        {
            if (GameObject.FindObjectOfType<Finisher.Cameras.CameraLookController>())
            {
                return GameObject.FindObjectOfType<Finisher.Cameras.CameraLookController>().gameObject.transform;
            }
            else if (Camera.main != null)
            {
                Debug.Log("Non Game Camera In Use");
                return Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.",
                    gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            return null;
        }

        #endregion Public Interface

        protected override void Start()
        {
            base.Start();

            Strafing = true;
            playerIP = GetComponent<PlayerMoveInputProcessor>();
            if (characterState.spawnConfig == null)
            {
                characterState.spawnConfig = new SpawnConfig();
            }
            GameObject indicator = Instantiate(CombatTargetIndicator);
            indicator.GetComponent<CombatTargetIndicatorController>().player = this;
        }

        private void Update()
        {
            if (characterState.Dying && Input.anyKey || Input.GetKeyDown(KeyCode.Y))
            {
                characterState.spawnConfig.runConfig();
            }
        }

        private Transform GetPlayerCombatTarget()
        {
            SetClosestEnemyColliders();

            if (enemyColliders.Count <= 0)
            {
                return null;
            }

            Transform target = null;
            target = FindPreferredEnemyTarget();

            return target;
        }

        private void SetClosestEnemyColliders()
        {
            int layerMask = 1 << LayerNames.EnemyLayer;
            enemyColliders = Physics.OverlapSphere(transform.position, MAINRANGE, layerMask).ToList();

            enemyColliders = enemyColliders.OrderBy(
                enemy => Vector2.Distance(this.transform.position, enemy.transform.position)
            ).ToList();
        }

        private Transform FindPreferredEnemyTarget()
        {
            Transform target = null;

            // ORDERED BY DISTANCE, NEAREST -> FARTHEST
            foreach (Collider enemyCollider in enemyColliders)
            {
                var targetState = enemyCollider.gameObject.GetComponent<CharacterState>();

                // checks for valid enemy
                if (enemyCollider.gameObject.GetComponent<CharacterMotor>() == null)
                {
                    continue;
                }

                if (targetState.Dying)
                {
                    continue;
                }

                // get the current angle of that enemy to the left or right of you
                Vector3 targetDir = enemyCollider.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);

                if (usingLSInput)
                {
                    // gets the angle between the current enemy and the LS stick direction
                    float overrideAngle = Vector3.Angle(targetDir, playerIP.InputMoveDirection);

                    //check that the enemy is in the directional hitbox
                    if (overrideAngle < DIRECTIONAL_INPUT_HITBOX)
                    {
                        target = enemyCollider.transform;
                        break;
                    }
                }

                // check if the enemy is in your main hitbox / camera field of view
                if (angle < MAINFOV)
                {
                    target = enemyCollider.transform;

                    if (!usingLSInput)
                        break;
                }

                // if the enemy is not in the directional or main hitboxes, and within
                // range and angle of the secondary of the secondary hitboxes
                if (Vector3.Distance(transform.position, enemyCollider.transform.position) <= SECONDARY_HITBOX_RANGE &&
                    angle < SECONDARY_HITBOX)
                {
                    if (target == null)
                        target = enemyCollider.transform;
                }
            }

            return target;
        }

        #region Class Overrides

        protected override void snapToGround()
        {
            rigidBody.AddForce(Vector3.down * 50, ForceMode.Acceleration);
        }

        #endregion Class Overrides

        // draws the hitbox lines for player, the 2 smaller hitboxes, and the player input hitbox
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

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

            Gizmos.color = Color.white;

            // local coordinate rotation around the Y axis to the given angle
            rotation = Quaternion.AngleAxis(SECONDARY_HITBOX, Vector3.up);
            // add the desired distance to the direction
            addDistanceToDirection = rotation * transform.forward * SECONDARY_HITBOX_RANGE;
            destination = transform.position + addDistanceToDirection;

            rotation2 = Quaternion.AngleAxis(-SECONDARY_HITBOX, Vector3.up);
            // add the desired distance to the direction
            addDistanceToDirection2 = rotation2 * transform.forward * SECONDARY_HITBOX_RANGE;
            destination2 = transform.position + addDistanceToDirection2;

            Gizmos.DrawLine(transform.position + Vector3.up, destination + Vector3.up);
            Gizmos.DrawLine(transform.position + Vector3.up, destination2 + Vector3.up);

            Gizmos.color = Color.red;

            if (playerIP)
            {
                // local coordinate rotation around the Y axis to the given angle
                rotation = Quaternion.AngleAxis(DIRECTIONAL_INPUT_HITBOX, Vector3.up);
                // add the desired distance to the direction
                addDistanceToDirection = rotation * playerIP.InputMoveDirection * MAINRANGE;
                destination = transform.position + addDistanceToDirection;

                rotation2 = Quaternion.AngleAxis(-DIRECTIONAL_INPUT_HITBOX, Vector3.up);
                // add the desired distance to the direction
                addDistanceToDirection2 = rotation2 * playerIP.InputMoveDirection * MAINRANGE;
                destination2 = transform.position + addDistanceToDirection2;

                Gizmos.DrawLine(transform.position + Vector3.up, destination + Vector3.up);
                Gizmos.DrawLine(transform.position + Vector3.up, destination2 + Vector3.up);
            }
        }
    }
}