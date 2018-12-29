using System.Linq;
using UnityEngine;

namespace Finisher.Characters
{
    public class PlayerCharacterController : CharacterAnimator
    {

        public Transform CombatTarget { get; private set; }
        public bool CombatTargetInRange { get; private set; } // tries to look at the set staffing target if true, matches camera rotation if false
        public bool Attacking { get { return combatSystem.IsAttacking; } }
        public bool Dodging { get { return combatSystem.IsDodging; } }

        [Header("Player Controller Specific Settings")]

        [SerializeField] float mainRange = 3f;
        [SerializeField] float extraRange = 1.5f;

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
            var tempCombatTarget = GetPlayerCombatTarget();
            if (tempCombatTarget)
            {
                CombatTargetInRange = true;
                CombatTarget = tempCombatTarget; // todo forget the current target if out of range
            }
            else
            {
                if (CombatTarget && CombatTarget.gameObject.GetComponent<CharacterMotor>().Dying)
                {
                    CombatTarget = null;
                }
                CombatTargetInRange = false;
            }
        }

        private Transform GetPlayerCombatTarget()
        {
            //Get nearby enemy colliders
            int layerMask = 1 << LayerNames.EnemyLayer;
            var enemyColliders = Physics.OverlapSphere(transform.position, mainRange, layerMask).ToList();
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
                float angle = Vector3.Angle(targetDir, transform.forward);

                // check if the enemy is in your field of vision
                float mainFOV = 90f;
                float extraFOV = 190f;
                if (angle < mainFOV / 2)
                {
                    //Debug.DrawLine(transform.position + Vector3.up, target.position + Vector3.up, Color.red);
                    return target;
                }
                else if (Vector3.Distance(transform.position, target.position) <= extraRange && angle < extraFOV / 2)
                {
                    if (alternateTarget == null)
                    {
                        alternateTarget = target;
                    }
                }
            }
            //if(alternateTarget)
            //Debug.DrawLine(transform.position + Vector3.up, alternateTarget.transform.position + Vector3.up, Color.white);
            return alternateTarget;
        }

        private void SetCharacterRotation()
        {
            if (Strafing)
            {
                if (CanRotate && playerI.HasMoveInput())
                {
                    if (CombatTargetInRange)
                    {
                        LookAtCombatTarget();
                    }
                    else
                    {
                        UseCamRigRotation();
                    }
                }
                else if (Dodging) {
                    UseCamRigRotation();
                }
                else if(Attacking)
                {
                    if (CombatTargetInRange)
                    {
                        LookAtCombatTarget();
                    }
                    else
                    {
                        UseCamRigRotation();
                    }
                }
            }
        }

        private void UseCamRigRotation()
        {
            transform.rotation = camRig.localRotation;
        }

        private void LookAtCombatTarget()
        {
            transform.LookAt(new Vector3(CombatTarget.position.x, transform.position.y, CombatTarget.position.z));
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
    }
}