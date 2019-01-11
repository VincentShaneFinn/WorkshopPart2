﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Finisher.Characters
{
    public class PlayerCharacterController : CharacterAnimator
    {

        #region Class Variables

        public Transform CombatTarget { get; private set; }
        public bool CombatTargetInRange { get; private set; } // tries to look at the set staffing target if true, matches camera rotation if false

        //[Header("Player Controller Specific")]

        [Header("Player Combat Target Hitbox Settings")]
        [SerializeField] private GameObject CombatTargetIndicator;
        [SerializeField] private float RotateWithCameraSpeed = 10f;
        [SerializeField] private float AutoLockTurnSpeed = 5f;
        [SerializeField] private float MAXCOMBATTARGETRANGE = 20f;
        [SerializeField] private float MAINRANGE = 3f;
        [SerializeField] private float EXTRARANGE = 1.5f;
        [SerializeField] private float MAINFOV = 50f;
        [SerializeField] private float EXTRAFOV = 100f;
        [SerializeField] private float DIRECTIONALINPUTFOV = 35f;

        private bool usingLSInput = false;
        private List<Collider> enemyColliders = null;
        private GameObject currentCombatTargetIndicator;

        private PlayerMoveInputProcessor playerIP;
        private Transform camRig = null;

        #endregion

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
                Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }
            return null;
        }

        #endregion

        void Start()
        {
            Strafing = true;
            playerIP = GetComponent<PlayerMoveInputProcessor>();
            camRig = GetMainCameraTransform();
        }

        void Update()
        {
            SetCurrentCombatTarget();
            UpdateCombatTargetIndicator();
            SetCharacterRotation();
        }

        #region SetCombatTarget

        private void SetCurrentCombatTarget()
        {
            usingLSInput = playerIP.InputMoveDirection != Vector3.zero;
            if (CombatTarget)
            {
                float distanceFromTarget = Vector3.Distance(CombatTarget.position, transform.position);
                if (CombatTarget.gameObject.GetComponent<CharacterState>().Dying ||
                    distanceFromTarget > MAXCOMBATTARGETRANGE)
                {
                    CombatTarget = null;
                    CombatTargetInRange = false;
                }
                else if (distanceFromTarget > MAINRANGE)
                {
                    CombatTargetInRange = false;
                }

            }
            if ((usingLSInput && characterState.Attacking && animator.IsInTransition(0)) ||
                !CombatTargetInRange)
            {
                SetNewCombatTarget();
            }
        }

        private void SetNewCombatTarget()
        {
            var tempCombatTarget = GetPlayerCombatTarget();
            if (tempCombatTarget)
            {
                CombatTargetInRange = true;
                CombatTarget = tempCombatTarget; // todo forget the current target if out of range
            }
            else
            {
                CombatTargetInRange = false;
            }
        }

        private Transform GetPlayerCombatTarget()
        {
            SetClosestEnemyColliders();

            if (enemyColliders.Count <= 0) { return null; }

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
            foreach (Collider enemyCollider in enemyColliders)
            {

                var targetState = enemyCollider.gameObject.GetComponent<CharacterState>();
                if (enemyCollider.gameObject.GetComponent<CharacterMotor>() == null) { continue; }
                else if (targetState.Dying) { continue; }

                // get the current angle of that enemy to the left or right of you
                Vector3 targetDir = enemyCollider.transform.position - transform.position;

                if (usingLSInput)
                {
                    float overrideAngle = Vector3.Angle(targetDir, playerIP.InputMoveDirection);

                    //first check for user directional Input
                    if (overrideAngle < DIRECTIONALINPUTFOV)
                    {
                        target = enemyCollider.transform;
                        break;
                    }
                }

                float angle = Vector3.Angle(targetDir, transform.forward);

                // check if the enemy is in your field of vision
                if (angle < MAINFOV)
                {
                    target = enemyCollider.transform;
                    if (!usingLSInput)
                    {
                        break;
                    }
                }
                else if (Vector3.Distance(transform.position, enemyCollider.transform.position) <= EXTRARANGE && angle < EXTRAFOV)
                {
                    if (target == null) // set the target to the one found in the smaller outer cones
                    {
                        target = enemyCollider.transform;
                    }
                }
            }

            return target;
        }

        #endregion

        #region UpdateCombatTargetIndicator

        private void UpdateCombatTargetIndicator()
        {
            if (CombatTarget)
            {
                if (!currentCombatTargetIndicator)
                {
                    currentCombatTargetIndicator = Instantiate(CombatTargetIndicator, CombatTarget);
                }
                else
                {
                    currentCombatTargetIndicator.transform.parent = CombatTarget;
                }
                currentCombatTargetIndicator.transform.localPosition = new Vector3(0, CombatTarget.GetComponent<CapsuleCollider>().height + .2f, 0);
            }
            else
            {
                if (currentCombatTargetIndicator)
                {
                    Destroy(currentCombatTargetIndicator);
                }
            }
        }

        #endregion

        #region SetCharacterRotation

        private void SetCharacterRotation()
        {
            if (characterState.Dying) {
                return;
            }

            if (characterState.Grabbing)
            {
                RotateWithCamRig(true);
                return;
            }

            if (Strafing)
            {
                if (characterState.Dodging) {
                    RotateWithCamRig();
                }
                else if(characterState.Attacking)
                {
                    if (CombatTargetInRange)
                    {
                        EngageEnemy();
                    }
                    else
                    {
                        RotateWithCamRig();
                    }
                }
                else if (CanRotate && playerIP.HasMoveInput())
                {
                    RotateWithCamRig();
                }
            }
            else
            {
                if (characterState.Dodging)
                {
                    RotateWithCamRig();
                }
            }
        }

        private void RotateWithCamRig(bool instant = false)
        {
            if (instant)
            {
                transform.rotation = camRig.localRotation;
            }
            else
            {
                // Smoothly rotate towards the target point.
                transform.rotation = Quaternion.Slerp(transform.rotation, camRig.localRotation, RotateWithCameraSpeed * Time.deltaTime);
            }
        }

        private void EngageEnemy()
        {
            var targetRotation = Quaternion.LookRotation(CombatTarget.transform.position - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, AutoLockTurnSpeed * Time.deltaTime);
        }

        #endregion

        #region Class Overrides

        protected override void snapToGround()
        {
            rigidBody.AddForce(Vector3.down * 50, ForceMode.Acceleration);
        }

        #endregion

        // draws the hitbox lines for player, the 2 smaller hitboxes, and the player input hitbox
        void OnDrawGizmos()
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
            rotation = Quaternion.AngleAxis(EXTRAFOV, Vector3.up);
            // add the desired distance to the direction
            addDistanceToDirection = rotation * transform.forward * EXTRARANGE;
            destination = transform.position + addDistanceToDirection;

            rotation2 = Quaternion.AngleAxis(-EXTRAFOV, Vector3.up);
            // add the desired distance to the direction
            addDistanceToDirection2 = rotation2 * transform.forward * EXTRARANGE;
            destination2 = transform.position + addDistanceToDirection2;

            Gizmos.DrawLine(transform.position + Vector3.up, destination + Vector3.up);
            Gizmos.DrawLine(transform.position + Vector3.up, destination2 + Vector3.up);

            Gizmos.color = Color.red;

            if (playerIP)
            {

                // local coordinate rotation around the Y axis to the given angle
                rotation = Quaternion.AngleAxis(DIRECTIONALINPUTFOV, Vector3.up);
                // add the desired distance to the direction
                addDistanceToDirection = rotation * playerIP.InputMoveDirection * MAINRANGE;
                destination = transform.position + addDistanceToDirection;

                rotation2 = Quaternion.AngleAxis(-DIRECTIONALINPUTFOV, Vector3.up);
                // add the desired distance to the direction
                addDistanceToDirection2 = rotation2 * playerIP.InputMoveDirection * MAINRANGE;
                destination2 = transform.position + addDistanceToDirection2;

                Gizmos.DrawLine(transform.position + Vector3.up, destination + Vector3.up);
                Gizmos.DrawLine(transform.position + Vector3.up, destination2 + Vector3.up);

            }

        }
    }
}