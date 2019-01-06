using UnityEngine;
using UnityEngine.UI;

using Finisher.Cameras;
using Finisher.Characters.Finishers;
using System;

namespace Finisher.Characters.Systems {

    public class FinisherSystem : MonoBehaviour
    {

        #region Class Variables

        public bool FinisherModeActive { get { return character.FinisherModeActive; } }
        public bool Grabbing { get { return character.Grabbing; } }
        public float CurrentVolatilityDamage
        {
            get
            {
                if (combatSystem.CurrentAttackType == AttackType.LightBlade)
                {
                    if (Grabbing)
                    {
                        return lightVolatilityDamage * 3;
                    }
                    else
                    {
                        return lightVolatilityDamage;
                    }
                }
                else
                {
                    if (Grabbing)
                    {
                        return heavyVolatilityDamage * 3;
                    }
                    else
                    {
                        return heavyVolatilityDamage;
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
                    return lightFinisherGain;
                }
                else
                {
                    return heavyFinisherGain;
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
        private PlayerCharacterController character;
        private CombatSystem combatSystem;
        private FreeLookCam freeLookCam;
        private Slider finisherMeter;
        private GameObject inFinisherIndicator;

        private bool L3Pressed = false;
        private bool R3Pressed = false;

        #region Finisher Settings

        [Header("Finisher Settings")]
        [SerializeField] private float maxFinisherMeter = 100f;
        [SerializeField] private float lightFinisherGain = 10f;
        [SerializeField] private float heavyFinisherGain = 30f;
        [SerializeField] private float lightVolatilityDamage = 10f;
        [SerializeField] private float heavyVolatilityDamage = 30f;

        #endregion

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
            character.OnCharacterKilled += stopGrab;
            combatSystem = GetComponent<CombatSystem>();
            freeLookCam = FindObjectOfType<FreeLookCam>();

            OnGrabbingTargetToggled += toggleGrab;
            OnFinisherModeToggled += toggleFinisherMode;

            GetComponent<PlayerHealthSystem>().OnKnockBack += ToggleGrabOff;

            finisherMeter = FindObjectOfType<UI.PlayerUIObjects>().FinisherSlider;

            inFinisherIndicator = FindObjectOfType<UI.PlayerUIObjects>().InFinisherIndicator.gameObject;
            inFinisherIndicator.gameObject.SetActive(false);

            decreaseFinisherMeter(maxFinisherMeter); // deplete finisher meter at start
        }


        void Update()
        {
            if (character.Dying)
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
                if (FinisherModeActive || currentFinisherMeter >= maxFinisherMeter - float.Epsilon)
                {
                    OnFinisherModeToggled(!FinisherModeActive);
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
            if (FinisherModeActive)
            {
                if (Input.GetButtonDown(InputNames.Grab))
                {
                    if (grabTarget)
                    {
                        OnGrabbingTargetToggled(false);
                    }
                    else if (character.CombatTarget != null && !character.Uninteruptable)
                    {
                        OnGrabbingTargetToggled(true);
                    }
                }
            }
        }

        private void attemptFinisher()
        {

            if (character.Grabbing)
            {
                var grabHealthSystem = grabTarget.GetComponent<EnemyHealthSystem>();

                if (grabHealthSystem &&
                    Input.GetButtonDown(InputNames.Finisher) && 
                    grabHealthSystem.GetVolaitilityAsPercent() >= 1f - Mathf.Epsilon)
                {
                    grabTarget.GetComponent<HealthSystem>().Kill();
                    character.SetTriggerOverride(AnimConstants.Parameters.INVULNERABLEACTION_TRIGGER, 
                        AnimConstants.OverrideIndexes.INVULNERABLEACTION_INDEX, 
                        flameAOE.AnimationToPlay);
                }
            }
        }

        #endregion

        private void aimingHandlerWithGrabTarget()
        {
            if (grabTarget)
            {
                if (grabTarget.GetComponent<CharacterMotor>().Dying)
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
                increaseFinisherMeter(maxFinisherMeter);
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
            if (!FinisherModeActive)
            {
                increaseFinisherMeter(amount);
            }
            checkFinisherFull();
        }

        private void increaseFinisherMeter(float amount)
        {
            currentFinisherMeter += amount;
            if(currentFinisherMeter > maxFinisherMeter - Mathf.Epsilon)
            {
                currentFinisherMeter = maxFinisherMeter;
            }
            updateFinisherMeterUI();
        }

        private void decreaseFinisherMeter(float amount)
        {
            currentFinisherMeter -= amount;
            if (currentFinisherMeter < Mathf.Epsilon)
            {
                currentFinisherMeter = 0;
                if (FinisherModeActive)
                {
                    OnFinisherModeToggled(false);
                }
            }
            updateFinisherMeterUI();
        }

        private void checkFinisherFull()
        {
            if (currentFinisherMeter >= maxFinisherMeter)
            {
                // do something if finisher full
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
            character.Grabbing = true;
            grabTarget.GetComponent<CharacterMotor>().Stunned = true;
        }

        private void stopGrab()
        {
            if (grabTarget)
            {
                grabTarget.GetComponent<CharacterMotor>().Stunned = false;
            }
            grabTarget = null;
            freeLookCam.NewFollowTarget = null;
            character.Grabbing = false;
        }

        #endregion

        // subscribed to the OnFinisherModeToggled delegate
        private void toggleFinisherMode(bool enabled)
        {
            var finisherModeActive = enabled;

            animator.SetBool(AnimConstants.Parameters.FINISHERMODE_BOOL, finisherModeActive);
            animator.SetTrigger(AnimConstants.Parameters.RESETPEACEFULLY_TRIGGER);
            inFinisherIndicator.gameObject.SetActive(finisherModeActive);

            if (!finisherModeActive)
            {
                ToggleGrabOff();
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
