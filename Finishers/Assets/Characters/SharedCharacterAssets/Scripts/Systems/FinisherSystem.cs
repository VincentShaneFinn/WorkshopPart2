using UnityEngine;
using UnityEngine.UI;

using Finisher.Cameras;
using Finisher.Characters.Skills;

namespace Finisher.Characters {
    public class FinisherSystem : MonoBehaviour
    {

        #region Class Variables

        public bool FinisherModeActive { get { return character.FinisherModeActive; } }

        public delegate void StartGrabbingTarget();
        public event StartGrabbingTarget OnStartGrabbingTarget;
        public delegate void StopGrabbingTarget();
        public event StopGrabbingTarget OnStopGrabbingTarget;

        private Transform grabTarget;
        private float currentFinisherMeter = 0;

        private Animator animator;
        private PlayerCharacterController character;
        private CombatSystem combatSystem;
        private FreeLookCam freeLookCam;
        private Slider finisherMeter;

        #region Finisher Settings

        [Header("Finisher Settings")]
        [SerializeField] private float maxFinisherMeter = 100f;

        #endregion

        #region Siphoning Skills // todo encapsualte into another class later

        [Header("Siphoning Settings")]
        [SerializeField] private ThrowingWeapon throwingWeapon;
        [SerializeField] private float distanceFromEnemyBack = .1f;

        #endregion

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

            getPlayerFinisherSlider();
            decreaseFinisherMeter(maxFinisherMeter);
        }

        private void getPlayerFinisherSlider()
        {
            if (gameObject.tag == "Player")
            {
                finisherMeter = FindObjectOfType<UI.PlayerUIObjects>().FinisherSlider;
            }
        }

        void Update()
        {
            if (character.Dying)
            {
                return;
            }

            finisherInputProcessing();
            aimingHandlerWithGrabTarget();
        }

        #region Update Helpers

        private void finisherInputProcessing()
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

        private void aimingHandlerWithGrabTarget()
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

        #region Public Interface

        #region FinisherMeter

        public void GainFinisherMeter(float amount)
        {
            increaseFinisherMeter(amount);
            checkFinisherFull();
        }

        private void increaseFinisherMeter(float amount)
        {
            currentFinisherMeter += amount;
            if(currentFinisherMeter > maxFinisherMeter - Mathf.Epsilon)
            {
                currentFinisherMeter = maxFinisherMeter;
            }
            finisherMeter.value = GetFinisherMeterAsPercent();
        }

        private void decreaseFinisherMeter(float amount)
        {
            currentFinisherMeter -= amount;
            if (currentFinisherMeter < Mathf.Epsilon)
            {
                currentFinisherMeter = 0;
            }
            finisherMeter.value = GetFinisherMeterAsPercent();
        }

        private void checkFinisherFull()
        {
            if (currentFinisherMeter >= maxFinisherMeter)
            {
                print("finisher Metere full");
            }
        }

        public float GetFinisherMeterAsPercent()
        {
            return currentFinisherMeter / maxFinisherMeter;
        }

        #endregion

        #region Stabbed Enemy

        public void StabbedEnemy(GameObject enemy)
        {
            if (character.Grabbing)
            {
                if (enemy == grabTarget.gameObject)
                {
                    if (combatSystem.CurrentAttackType == AttackType.LightBlade)
                    {
                        throwSword(enemy);
                    }
                    else if (combatSystem.CurrentAttackType == AttackType.HeavyBlade)
                    {
                        throwSwords(enemy);
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

        private void throwSword(GameObject enemy)
        {
            float spawnDistanceFromEnemy = enemy.GetComponent<CapsuleCollider>().radius + distanceFromEnemyBack;
            Vector3 targetSpawnPoint = enemy.transform.position + Vector3.up - (enemy.transform.forward * spawnDistanceFromEnemy);
            ThrowingWeapon currentThrowingWeapon = Instantiate(throwingWeapon, targetSpawnPoint, freeLookCam.transform.rotation);

            currentThrowingWeapon.ThrowWeapon();
        }

        private void throwSwords(GameObject enemy)
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

        #endregion

        #endregion
    }
}
