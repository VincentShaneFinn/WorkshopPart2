using UnityEngine;

using Finisher.Cameras;
using Finisher.Characters.Skills;

namespace Finisher.Characters {
    public class FinisherSystem : MonoBehaviour
    {
        private Animator animator;
        private PlayerCharacterController character;
        private FreeLookCam freeLookCam;
        private Transform grabTarget;

        public delegate void StartGrabbingTarget();
        public event StartGrabbingTarget OnStartGrabbingTarget;
        public delegate void StopGrabbingTarget();
        public event StopGrabbingTarget OnStopGrabbingTarget;

        #region Siphoning Skills // todo encapsualte into another class later

        [Header("Siphoning Settings")]
        [SerializeField] private ThrowingWeapon throwingWeapon;

        #endregion

        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<PlayerCharacterController>();
            character.OnCharacterKilled += stopGrab;
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

            MovementHandlerWithGrabTarget();
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

        private void MovementHandlerWithGrabTarget()
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
            grabTarget.GetComponent<CharacterMotor>().Staggered = false;
            grabTarget = null;
            freeLookCam.NewFollowTarget = null;
            character.Grabbing = false;
        }

        #endregion

        public void WeaponStruckEnemy(GameObject enemy)
        {
            if (character.FinisherModeActive)
            {
                if (character.Grabbing)
                {
                    if (enemy == grabTarget.gameObject)
                    {
                        float spawnDistanceFromEnemy = enemy.GetComponent<CapsuleCollider>().radius + 1f;
                        Vector3 targetSpawnPoint = grabTarget.position + Vector3.up - (grabTarget.forward * spawnDistanceFromEnemy);
                        ThrowingWeapon currentThrowingWeapon = Instantiate(throwingWeapon, targetSpawnPoint, freeLookCam.transform.rotation);

                        currentThrowingWeapon.ThrowWeapon();
                    }
                }
                else
                {
                    print("hit enemy in Finisher " + enemy);
                }

            }
        }
    }
}
