using UnityEngine;
using UnityEngine.UI;

using Finisher.Cameras;
using Finisher.Characters.Finishers;
using Finisher.Core;
using Finisher.Characters.Weapons;

namespace Finisher.Characters.Systems {

    public class FinisherSystem : MonoBehaviour
    {

        #region Class Variables

        public float CurrentVolatilityDamage
        {
            get
            {
                if (combatSystem.CurrentAttackType == AttackType.LightBlade)
                {
                    if (characterState.Grabbing)
                    {
                        return config.LightVolatilityDamage * 3;
                    }
                    else
                    {
                        return config.LightVolatilityDamage;
                    }
                }
                else
                {
                    if (characterState.Grabbing)
                    {
                        return config.HeavyVolatilityDamage * 3;
                    }
                    else
                    {
                        return config.HeavyVolatilityDamage;
                    }
                }
            }
        }
        public float CurrentFinisherGain
        {
            get
            {
                if (combatSystem.CurrentAttackType == AttackType.LightBlade)
                {
                    return config.LightFinisherGain;
                }
                else
                {
                    return config.HeavyFinisherGain;
                }
            }
        }

        #region Delegates

        public delegate void GrabbingTargetChanged(bool enabled);
        public event GrabbingTargetChanged OnGrabbingTargetToggled;

        public delegate void FinisherModeChanged(bool enabled);
        public event FinisherModeChanged OnFinisherModeToggled;

        #endregion

        private Transform grabTarget;
        private float currentFinisherMeter = 0;

        private Animator animator;
        private CharacterState characterState;
        private PlayerCharacterController character;
        private CombatSystem combatSystem;
        private CameraLookController freeLookCam;
        private Slider finisherMeter;

        private GameObject inFinisherIndicator;

        private Sword sword;
        private Knife knife;
        private enum WeaponToggle { Sword, Knife };

        private bool L3Pressed = false;
        private bool R3Pressed = false;

        [SerializeField] private FinisherConfig config;

        #region Siphoning Skills // todo encapsualte into another class later

        [Header("Siphoning Settings")]
        [SerializeField] private ThrowingWeapon throwingWeapon;
        [SerializeField] private float distanceFromEnemyBack = .1f;
        [SerializeField] private Flamethrower flamethrower;
        [SerializeField] private FlameAOE flameAOE;

        #endregion

        #endregion

        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<PlayerCharacterController>();
            combatSystem = GetComponent<CombatSystem>();
            freeLookCam = FindObjectOfType<CameraLookController>();

            knife = GetComponentInChildren<Knife>();
            sword = GetComponentInChildren<Sword>();
            toggleWeapon(WeaponToggle.Sword);

            finisherMeter = FindObjectOfType<UI.PlayerUIObjects>().FinisherSlider;

            inFinisherIndicator = FindObjectOfType<UI.PlayerUIObjects>().InFinisherIndicator.gameObject;
            inFinisherIndicator.gameObject.SetActive(false);

            decreaseFinisherMeter(config.MaxFinisherMeter); // deplete finisher meter at start
        }

        void OnEnable()
        {
            characterState = GetComponent<CharacterState>();
            characterState.DyingState.SubscribeToDeathEvent(stopGrab);

            OnGrabbingTargetToggled += toggleGrab;
            OnFinisherModeToggled += toggleFinisherMode;
            GetComponent<PlayerHealthSystem>().OnKnockBack += ToggleGrabOff;
        }

        void OnDisable()
        {
            characterState.DyingState.UnsubscribeToDeathEvent(stopGrab);

            OnGrabbingTargetToggled -= toggleGrab;
            OnFinisherModeToggled -= toggleFinisherMode;
            GetComponent<PlayerHealthSystem>().OnKnockBack -= ToggleGrabOff;
        }

        void Update()
        {
            if (characterState.Dying || GameManager.instance.GamePaused)
            {
                return;
            }

            testInput();

            finisherInputProcessing();
            aimingHandlerWithGrabTarget();
        }

        #region Update Helpers

        #region Finisher Input Processing

        private void finisherInputProcessing()
        {
            attemptToggleGrab();
            attemptFinisher();

            setL3AndR3();
            attemptToggleFinisherMode();
        }

        private void attemptToggleFinisherMode()
        {
            if (L3Pressed && R3Pressed)
            {
                L3Pressed = false;
                R3Pressed = false;
                if (characterState.FinisherModeActive || currentFinisherMeter >= config.MaxFinisherMeter - float.Epsilon)
                {
                    OnFinisherModeToggled(!characterState.FinisherModeActive);
                }
            }
        }

        private void setL3AndR3()
        {
            if (!L3Pressed && Input.GetButtonDown(InputNames.L3))
            {
                L3Pressed = true;
            }
            if (!R3Pressed && Input.GetButtonDown(InputNames.R3))
            {
                R3Pressed = true;
            }
            if (L3Pressed && Input.GetButtonUp(InputNames.L3))
            {
                L3Pressed = false;
            }
            if (R3Pressed && Input.GetButtonUp(InputNames.R3))
            {
                R3Pressed = false;
            }
        }

        private void attemptToggleGrab()
        {
            if (characterState.FinisherModeActive)
            {
                if (Input.GetButtonDown(InputNames.Grab))
                {
                    if (grabTarget)
                    {
                        OnGrabbingTargetToggled(false);
                    }
                    else if (character.CombatTarget != null && !characterState.Uninteruptable)
                    {
                        OnGrabbingTargetToggled(true);
                    }
                }
            }
        }

        private void attemptFinisher()
        {

            if (characterState.Grabbing)
            {
                var grabHealthSystem = grabTarget.GetComponent<EnemyHealthSystem>();

                if (grabHealthSystem &&
                    Input.GetButtonDown(InputNames.Finisher) && 
                    grabHealthSystem.GetVolaitilityAsPercent() >= 1f - Mathf.Epsilon)
                {
                    grabTarget.GetComponent<HealthSystem>().Kill();
                    characterState.EnterInvulnerableActionState(flameAOE.AnimationToPlay);
                }
            }
        }

        #endregion

        private void aimingHandlerWithGrabTarget()
        {
            if (grabTarget)
            {
                if (grabTarget.GetComponent<CharacterState>().Dying)
                {
                    OnGrabbingTargetToggled(false);
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

        private void testInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                increaseFinisherMeter(config.MaxFinisherMeter);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && grabTarget)
            {
                grabTarget.GetComponent<EnemyHealthSystem>().DamageVolatility(100f);
            }
        }

        #endregion

        #region Public Interface

        #region FinisherMeter

        public void GainFinisherMeter(float amount)
        {
            if (!characterState.FinisherModeActive)
            {
                increaseFinisherMeter(amount);
            }
            checkFinisherFull();
        }

        private void increaseFinisherMeter(float amount)
        {
            currentFinisherMeter += amount;
            if(currentFinisherMeter > config.MaxFinisherMeter - Mathf.Epsilon)
            {
                currentFinisherMeter = config.MaxFinisherMeter;
            }
            updateFinisherMeterUI();
        }

        private void decreaseFinisherMeter(float amount)
        {
            currentFinisherMeter -= amount;
            if (currentFinisherMeter < Mathf.Epsilon)
            {
                currentFinisherMeter = 0;
                if (characterState.FinisherModeActive)
                {
                    OnFinisherModeToggled(false);
                }
            }
            updateFinisherMeterUI();
        }

        private void checkFinisherFull()
        {
            if (currentFinisherMeter >= config.MaxFinisherMeter)
            {
                // do something if finisher full
            }
        }

        public float GetFinisherMeterAsPercent()
        {
            return currentFinisherMeter / config.MaxFinisherMeter;
        }

        #endregion

        #region Stabbed Enemy

        public void StabbedEnemy(GameObject enemy)
        {
            if (characterState.Grabbing)
            {
                if (enemy == grabTarget.gameObject)
                {
                    if (combatSystem.CurrentAttackType == AttackType.LightBlade)
                    {
                        throwFlames(enemy);
                        decreaseFinisherMeter(10f);
                    }
                    else if (combatSystem.CurrentAttackType == AttackType.HeavyBlade)
                    {
                        throwSword(enemy);
                        decreaseFinisherMeter(25f);
                    }
                }
            }
        }

        private void throwSword(GameObject enemy)
        {
            float spawnDistanceFromEnemy = enemy.GetComponent<CapsuleCollider>().radius + distanceFromEnemyBack;
            Vector3 targetSpawnPoint = enemy.transform.position + Vector3.up - (enemy.transform.forward * spawnDistanceFromEnemy);
            ThrowingWeapon currentThrowingWeapon = Instantiate(throwingWeapon, targetSpawnPoint, freeLookCam.transform.rotation);

            currentThrowingWeapon.ThrowWeapon();
        }

        private void throwFlames(GameObject enemy)
        {
            float spawnDistanceFromEnemy = enemy.GetComponent<CapsuleCollider>().radius + 1f;
            Vector3 targetSpawnPoint = enemy.transform.position + Vector3.up - (enemy.transform.forward * spawnDistanceFromEnemy);
            Instantiate(flamethrower, targetSpawnPoint, freeLookCam.transform.rotation);
        }

        #endregion

        #endregion

        #region Animation Events

        private void PerformFinisherSkill()
        {
            decreaseFinisherMeter(50f);
            Instantiate(flameAOE, transform.position, transform.rotation);
        }

        #endregion

        #region Toggle Grab Delegate Method

        public void ToggleGrabOff()
        {
            OnGrabbingTargetToggled(false);
        }

        // subscribed to the OnGrabbingToggled()
        private void toggleGrab(bool enabled)
        {
            if (enabled)
            {
                startGrab();
            }
            else
            {
                stopGrab();
            }
        }

        private void startGrab()
        {
            grabTarget = character.CombatTarget;
            freeLookCam.NewFollowTarget = grabTarget;
            characterState.Grabbing = true;
            grabTarget.GetComponent<CharacterState>().Stunned = true;
        }

        private void stopGrab()
        {
            if (grabTarget)
            {
                grabTarget.GetComponent<CharacterState>().Stunned = false;
            }
            grabTarget = null;
            freeLookCam.NewFollowTarget = null;
            characterState.Grabbing = false;
        }

        #endregion

        // subscribed to the OnFinisherModeToggled delegate
        private void toggleFinisherMode(bool enabled)
        {
            var finisherModeActive = enabled;

            animator.SetBool(AnimConstants.Parameters.FINISHERMODE_BOOL, finisherModeActive);
            animator.SetTrigger(AnimConstants.Parameters.RESETPEACEFULLY_TRIGGER);
            inFinisherIndicator.gameObject.SetActive(finisherModeActive);

            if (enabled)
            {
                toggleWeapon(WeaponToggle.Knife);
            }
            else
            {
                toggleWeapon(WeaponToggle.Sword);
            }

            if (!finisherModeActive)
            {
                ToggleGrabOff();
            }
        }

        private void toggleWeapon(WeaponToggle weaponToggle)
        {
            knife.gameObject.SetActive(false);
            sword.gameObject.SetActive(false);

            switch (weaponToggle)
            {
                case WeaponToggle.Sword:
                    sword.gameObject.SetActive(true);
                    break;
                case WeaponToggle.Knife:
                    knife.gameObject.SetActive(true);
                    break;
            }
        }

        private void updateFinisherMeterUI()
        {
            if (finisherMeter)
            {
                finisherMeter.value = GetFinisherMeterAsPercent();
            }
        }
    }
}
