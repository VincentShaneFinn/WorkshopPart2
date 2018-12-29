﻿using System.Linq;
using UnityEngine;

namespace Finisher.Characters
{
    public class PlayerCharacterController : CharacterAnimator
    {

        public Transform CombatTarget { get; private set; }
        public bool CombatTargetInRange { get; private set; } // tries to look at the set staffing target if true, matches camera rotation if false
        public bool Attacking { get { return combatSystem.IsAttacking; } }
        public bool Dodging { get { return combatSystem.IsDodging; } }

        [Header("Player Auto Target Hitbox Settings")]

        [SerializeField] private float AutoLockTurnSpeed = 5f;
        [SerializeField] private float MAINRANGE = 3f;
        [SerializeField] private float EXTRARANGE = 1.5f;
        [SerializeField] private float MAINFOV = 90f;
        [SerializeField] private float EXTRAFOV = 190f;

        private bool useDirectInput = false;

        private PlayerInputProcessor playerI;
        private CombatSystem combatSystem;
        private Transform camRig = null;
        

        #region Public Interface

        public Transform GetMainCameraTransform()
        {
            if (GameObject.FindObjectOfType<Finisher.Cameras.FreeLookCam>())
            {
                return GameObject.FindObjectOfType<Finisher.Cameras.FreeLookCam>().gameObject.transform;
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
            playerI = GetComponent<PlayerInputProcessor>();
            combatSystem = GetComponent<CombatSystem>();
            camRig = GetMainCameraTransform();
        }

        void Update()
        {
            SetCurrentCombatTarget();
            SetCharacterRotation();
        }

        private void SetCurrentCombatTarget()
        {
            useDirectInput = playerI.InputMoveDirection != Vector3.zero;
            if (CombatTarget)
            {
                if (CombatTarget.gameObject.GetComponent<CharacterMotor>().Dying)
                {
                    CombatTarget = null;
                    CombatTargetInRange = false;
                }
                else if (Vector3.Distance(CombatTarget.position, transform.position) > MAINRANGE)
                {
                    CombatTargetInRange = false;
                }
                if (useDirectInput && Attacking && Animator.IsInTransition(0))
                {
                    GetNewCombatTarget();
                }
            }
            if(!CombatTargetInRange)
            {
                GetNewCombatTarget();
            }

        }

        private void GetNewCombatTarget()
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
            //Get nearby enemy colliders
            int layerMask = 1 << LayerNames.EnemyLayer;
            var enemyColliders = Physics.OverlapSphere(transform.position, MAINRANGE, layerMask).ToList();
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

                if (useDirectInput)
                {
                    float overrideAngle = Vector3.Angle(targetDir, playerI.InputMoveDirection);

                    //first check for user directional Input
                    if (overrideAngle < 30f)
                    {
                        alternateTarget = target;
                        break;
                    }
                }

                float angle = Vector3.Angle(targetDir, transform.forward);

                // check if the enemy is in your field of vision
                if (angle < MAINFOV)
                {
                    alternateTarget = target;
                    if (!useDirectInput)
                    {
                        break;
                    }
                }
                else if (Vector3.Distance(transform.position, target.position) <= EXTRARANGE && angle < EXTRAFOV)
                {
                    if (alternateTarget == null)
                    {
                        alternateTarget = target;
                    }
                }
            }
            return alternateTarget;
        }

        private void SetCharacterRotation()
        {
            if (Strafing)
            {
                if (Dodging) {
                    RotateWithCamRig();
                }
                else if(Attacking)
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
                else if (CanRotate && playerI.HasMoveInput())
                {
                    RotateWithCamRig();
                }
            }
            else
            {
                if (Dodging)
                {
                    RotateWithCamRig();
                }
            }
        }

        private void RotateWithCamRig()
        {
            transform.rotation = camRig.localRotation;
        }

        private void EngageEnemy()
        {
            //transform.LookAt(new Vector3(CombatTarget.position.x, transform.position.y, CombatTarget.position.z));
            var targetRotation = Quaternion.LookRotation(CombatTarget.transform.position - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, AutoLockTurnSpeed * Time.deltaTime);
        }


        #region Class Overrides

        protected override void snapToGround()
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
            {
                transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
            }
        }

        #endregion

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

        }
    }
}