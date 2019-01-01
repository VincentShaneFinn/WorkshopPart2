using UnityEngine;

using Finisher.Cameras;
using Finisher.Characters.Skills;

namespace Finisher.Characters {
    public class FinisherSystem : MonoBehaviour
    {
        public bool FinisherModeActive { get { return character.FinisherModeActive; } }

        private Transform grabTarget;

        public delegate void StartGrabbingTarget();
        public event StartGrabbingTarget OnStartGrabbingTarget;
        public delegate void StopGrabbingTarget();
        public event StopGrabbingTarget OnStopGrabbingTarget;

        private Animator animator;
        private PlayerCharacterController character;
        private CombatSystem combatSystem;
        private FreeLookCam freeLookCam;

        #region Siphoning Skills // todo encapsualte into another class later

        [Header("Siphoning Settings")]
        [SerializeField] private ThrowingWeapon throwingWeapon;
        [SerializeField] private float distanceFromEnemyBack = .1f;

        #endregion

        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<PlayerCharacterController>();
            character.OnCharacterKilled += stopGrab;
            combatSystem = GetComponent<CombatSystem>();
            freeLookCam = FindObjectOfType<FreeLookCam>();
            OnStartGrabbingTarget += startGrab;
            OnStopGrabbingTarget += stopGrab;
        }

        void Update()
        {
            if (character.Dying)
            {
                return;
            }

            FinisherInputProcessing();

            AimingHandlerWithGrabTarget();
        }

        #region Update Helpers

        private void FinisherInputProcessing()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                animator.SetBool(AnimContstants.Parameters.FINISHERMODE_BOOL, !character.FinisherModeActive);
            }
            if (character.FinisherModeActive)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    if (grabTarget)
                    {
                        OnStopGrabbingTarget();
                    }
                    else
                    {
                        if (character.CombatTarget != null)
                        {
                            OnStartGrabbingTarget();
                        }
                    }
                }
            }
        }

        private void AimingHandlerWithGrabTarget()
        {
            if (grabTarget)
            {
                if (grabTarget.GetComponent<CharacterMotor>().Dying)
                {
                    OnStopGrabbingTarget();
                }
                else
                {
                    transform.position = grabTarget.position + grabTarget.forward;

                    Vector3 rot = freeLookCam.transform.rotation.eulerAngles;
                    rot = new Vector3(rot.x, rot.y + 180, rot.z);
                    grabTarget.rotation = Quaternion.Euler(rot);
                }
            }
        }

        #endregion

        #region Start and Stop grab

        private void startGrab()
        {
            grabTarget = character.CombatTarget;
            freeLookCam.NewFollowTarget = grabTarget;
            character.Grabbing = true;
            grabTarget.GetComponent<CharacterMotor>().Staggered = true;
        }

        private void stopGrab()
        {
            if (grabTarget)
            {
                grabTarget.GetComponent<CharacterMotor>().Staggered = false;
            }
            grabTarget = null;
            freeLookCam.NewFollowTarget = null;
            character.Grabbing = false;
        }

        #endregion

        public void StabbedEnemy(GameObject enemy)
        {
            if (character.Grabbing)
            {
                if (enemy == grabTarget.gameObject)
                {
                    if (combatSystem.CurrentAttackType == AttackType.LightBlade)
                    {
                        ThrowSword(enemy);
                    }
                    else if (combatSystem.CurrentAttackType == AttackType.HeavyBlade)
                    {
                        ThrowSwords(enemy);
                    }
                }
            }
            //else
            //{
            //    if (combatSystem.CurrentAttackType == AttackType.LightBlade)
            //    {
            //        ThrowSword(enemy);
            //    }
            //    else if (combatSystem.CurrentAttackType == AttackType.HeavyBlade)
            //    {
            //        ThrowSwords(enemy);
            //    }
            //}
        }

        private void ThrowSword(GameObject enemy)
        {
            float spawnDistanceFromEnemy = enemy.GetComponent<CapsuleCollider>().radius + distanceFromEnemyBack;
            Vector3 targetSpawnPoint = enemy.transform.position + Vector3.up - (enemy.transform.forward * spawnDistanceFromEnemy);
            ThrowingWeapon currentThrowingWeapon = Instantiate(throwingWeapon, targetSpawnPoint, freeLookCam.transform.rotation);

            currentThrowingWeapon.ThrowWeapon();
        }

        private void ThrowSwords(GameObject enemy)
        {
            float spawnDistanceFromEnemy = enemy.GetComponent<CapsuleCollider>().radius + 1f;
            Vector3 targetSpawnPoint = enemy.transform.position + Vector3.up - (enemy.transform.forward * spawnDistanceFromEnemy);
            ThrowingWeapon currentThrowingWeapon = Instantiate(throwingWeapon, targetSpawnPoint, freeLookCam.transform.rotation);
            currentThrowingWeapon.ThrowWeapon();

            targetSpawnPoint = enemy.transform.position + Vector3.up - (enemy.transform.forward * spawnDistanceFromEnemy) + enemy.transform.right;
            currentThrowingWeapon = Instantiate(throwingWeapon, targetSpawnPoint, freeLookCam.transform.rotation);
            currentThrowingWeapon.ThrowWeapon();

            targetSpawnPoint = enemy.transform.position + Vector3.up - (enemy.transform.forward * spawnDistanceFromEnemy) - enemy.transform.right;
            currentThrowingWeapon = Instantiate(throwingWeapon, targetSpawnPoint, freeLookCam.transform.rotation);
            currentThrowingWeapon.ThrowWeapon();
        }
    }
}
