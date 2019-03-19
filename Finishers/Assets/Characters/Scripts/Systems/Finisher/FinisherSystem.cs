using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Finisher.Cameras;
using Finisher.Core;
using Finisher.Characters.Weapons; //TODO: Consider a way to do it via animation layer
using Finisher.Characters.Player; //TODO: Consider rewire or player specific, and Enemy Specific Finisher Systems
using Finisher.Characters.Player.Finishers;
using Finisher.Characters.Enemies.Systems;
using Finisher.Characters.Systems.Strategies;
using Finisher.UI.Meters;
using System;

namespace Finisher.Characters.Systems {

    public class FinisherSystem : MonoBehaviour
    {

        #region Class Variables

        [SerializeField] private FinisherConfig config;
        [SerializeField] private FinisherModeDamageSystem lightFinisherAttackDamageSystem;
        [SerializeField] private FinisherModeDamageSystem heavyFinisherAttackDamageSystem;
        private GameObject FinisherGuidePanel;

        #region Delegates

        public delegate void GrabbingTargetChanged(bool enabled);
        public event GrabbingTargetChanged OnGrabbingTargetToggled;

        public delegate void FinisherModeChanged(bool enabled);
        public event FinisherModeChanged OnFinisherModeToggled;

        #endregion

        private Transform grabTarget;
        private float currentFinisherMeter = 0;
        private FinisherExecution currentFinisherExecution;

        private Animator animator;
        private AnimOverrideSetter animOverrideSetter;
        private CharacterState characterState;
        private PlayerCharacterController character;
        private CombatSystem combatSystem;
        private CameraLookController freeLookCam;
        private UI_FinisherMeter finisherMeter;

        private GameObject inFinisherIndicator;

        private Sword sword;
        private Knife knife;
        private Armblade armblade;
        private enum WeaponToggle { Sword, Knife ,SoulSword};

        private bool L3Pressed = false;
        private bool R3Pressed = false;
        public Tutorial finishTutorial;

        #region Siphoning Skills // todo encapsualte into another class later

        [Header("Siphoning Settings")]
        [SerializeField] private ThrowingWeapon throwingWeapon;
        [SerializeField] private float distanceFromEnemyBack = .1f;
        [SerializeField] private PulseBlast flamethrower;
        [SerializeField] private FlameAOE flameAOE;
        [SerializeField] private SoulInfusion soulInfusion;
        [SerializeField] private Blades blades;
        [SerializeField] private StunAOE stunAOE;
        [SerializeField] private StunAOE grabStun;
        [SerializeField] private float soulSwordTime=10;

        #endregion

        #endregion


        void Start()
        {
            animator = GetComponent<Animator>();
            animOverrideSetter = GetComponent<AnimOverrideSetter>();
            character = GetComponent<PlayerCharacterController>();
            combatSystem = GetComponent<CombatSystem>();
            freeLookCam = FindObjectOfType<CameraLookController>();

            subscribeToDelegates();

            knife = GetComponentInChildren<Knife>();
            sword = GetComponentInChildren<Sword>();
            armblade = GetComponentInChildren<Armblade>();
            toggleWeapon(WeaponToggle.Sword);

            finisherMeter = FindObjectOfType<UI.PlayerUIObjects>().gameObject.GetComponentInChildren<UI_FinisherMeter>();
            FinisherGuidePanel = FindObjectOfType<UI.PlayerUIObjects>().FinisherGuidePanel;

            inFinisherIndicator = FindObjectOfType<UI.PlayerUIObjects>().InFinisherIndicator.gameObject;
            inFinisherIndicator.gameObject.SetActive(false);

            decreaseFinisherMeter(config.MaxFinisherMeter); // deplete finisher meter at start
        }

        private void subscribeToDelegates()
        {
            characterState = GetComponent<CharacterState>();
            characterState.DyingState.SubscribeToDeathEvent(stopGrab);

            OnGrabbingTargetToggled += toggleGrab;
            OnFinisherModeToggled += toggleFinisherMode;
            GetComponent<HealthSystem>().OnKnockBack += ToggleGrabOff;

            combatSystem.OnHitEnemy += GainFinisherMeter;
        }

        void OnDestroy()
        {
            characterState.DyingState.UnsubscribeToDeathEvent(stopGrab);

            OnGrabbingTargetToggled -= toggleGrab;
            OnFinisherModeToggled -= toggleFinisherMode;
            GetComponent<HealthSystem>().OnKnockBack -= ToggleGrabOff;

            combatSystem.OnHitEnemy -= GainFinisherMeter;
        }
        private void soulOn() {
            sword.soulOn();
            knife.soulOn();
            armblade.soulOn();
        }
        private void soulOff() {
            sword.soulOff();
            knife.soulOff();
            armblade.soulOff();
        }
        void Update()
        {
            if (characterState.Dying || GameManager.instance.GamePaused)
            {
                return;
            }

            testInput();

            if (characterState.Grabbing && grabTarget == null)
            {
                OnGrabbingTargetToggled(false);
            }

            finisherInputProcessing();
            aimingHandlerWithGrabTarget();
        }

        #region Update Helpers

        #region Finisher Input Processing

        private void finisherInputProcessing()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.INVULNERABLE_SEQUENCE_TAG) &&
                !animator.IsInTransition(0))
            {
                attemptToggleGrab();
                attemptFinisher();
                attempQuickFinisher();
                setL3AndR3();
                attemptToggleFinisherMode();
            }
            attempFinisherSelection();
        }

        private void attemptToggleGrab()
        {
            if (characterState.FinisherModeActive)
            {
                if (FinisherInput.Grab())
                {
                    if (grabTarget)
                    {
                        OnGrabbingTargetToggled(false);
                    }
                    else if (character.CombatTarget != null && !characterState.Uninteruptable)
                    {
                        animator.SetTrigger(AnimConstants.Parameters.GRAB_TRIGGER);
                    }
                }
            }
        }

        bool isFinishing = false;

        private void attemptFinisher()
        {

            if (characterState.Grabbing && 
                !animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.FINISHER_EXECUTE_STATE) && 
                !animator.IsInTransition(0))
            {
                var grabHealthSystem = grabTarget.GetComponent<EnemyHealthSystem>();

                if (grabHealthSystem &&
                    //FinisherInput.Finisher() && 
                    !isFinishing &&
                    grabHealthSystem.GetIsFinishable())
                {
                    isFinishing = true;
                    animator.SetTrigger(AnimConstants.Parameters.RESETFORCEFULLY_TRIGGER);
                    animator.SetTrigger(AnimConstants.Parameters.FINISHER_EXECUTION_TRIGGER);
                    //Set the default finisher to play
                    overrideFinisherExecution(flameAOE);
                    toggleWeapon(WeaponToggle.Sword);
                }
            }
        }

        private void attempFinisherSelection()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(AnimConstants.States.FINISHER_SELECTION_STATE)) {
                FinisherGuidePanel.SetActive(true);
                if (FinisherInput.Finisher1())
                {
                    overrideFinisherExecution(flameAOE, true);
                }
                else if (FinisherInput.Finisher2())
                {
                    overrideFinisherExecution(soulInfusion, true);
                }
                else if (FinisherInput.Finisher4())
                {
                    overrideFinisherExecution(blades, true);
                }
            }
            else
            {
                FinisherGuidePanel.SetActive(false);
            }
        }

        private void overrideFinisherExecution(FinisherExecution finisherExecution, bool performSkill = false)
        {
            currentFinisherExecution = finisherExecution;
            animOverrideSetter.SetOverride(AnimConstants.OverrideIndexes.FINISHER_ACTIVATION_INDEX, currentFinisherExecution.AnimationToPlay);
            if (performSkill)
            {
                animator.SetTrigger(AnimConstants.Parameters.FINISHER_EXECUTION_TRIGGER);
            }
        }

        private bool pressedBoth = false;

        private void setL3AndR3()
        {
            if (Input.GetButtonDown(InputNames.L3))
            {
                L3Pressed = true;
            }
            if (Input.GetButtonDown(InputNames.R3))
            {
                R3Pressed = true;
            }
            if (L3Pressed)
            {
                StartCoroutine(freeL3());
            }
            if (R3Pressed)
            {
                StartCoroutine(freeR3());
            }
        }

        IEnumerator freeL3()
        {
            yield return new WaitForSeconds(.2f);
            L3Pressed = false;
        }

        IEnumerator freeR3()
        {
            yield return new WaitForSeconds(.2f);
            L3Pressed = false;
        }

        private void attemptToggleFinisherMode()
        {
            if (L3Pressed && R3Pressed && !pressedBoth)
            {
                L3Pressed = false;
                R3Pressed = false;
                if (characterState.FinisherModeActive || currentFinisherMeter >= config.MaxFinisherMeter - float.Epsilon)
                {
                    OnFinisherModeToggled(!characterState.FinisherModeActive);
                }
            }
        }

        private bool inQuickFinisher = false; //TODO: must make a better quick finisher system
        private void attempQuickFinisher()
        {
            if (FinisherInput.Finisher() && !characterState.Grabbing && characterState.FinisherModeActive && !inQuickFinisher)
            {
                var enemies = getEnemiesInFront();
                var enemyToFinish = getEnemyToQuickFinish(enemies);
                if (enemyToFinish)
                {
                    currentFinisherExecution = stunAOE;
                    combatSystem.LightAttack();
                    inQuickFinisher = true;
                    StartCoroutine(transformOvertime(enemyToFinish.transform));
                }
            }
        }

        private List<Collider> getEnemiesInFront()
        {
            int layerMask = 1 << LayerNames.EnemyLayer;
            var enemyColliders = Physics.OverlapSphere(transform.position, 2f, layerMask).ToList();

            enemyColliders = enemyColliders.OrderBy(
                enemy => Vector2.Distance(this.transform.position, enemy.transform.position)
                ).ToList();

            return enemyColliders;
        }

        private HealthSystem getEnemyToQuickFinish(List<Collider> enemies)
        {
            foreach (var enemy in enemies)
            {
                EnemyHealthSystem enemyHealthSystem = enemy.GetComponent<EnemyHealthSystem>();
                if (enemyHealthSystem && enemyHealthSystem.GetIsFinishable())
                {
                    return enemy.GetComponent<HealthSystem>();
                }
            }

            return null;
        }

        //TODO: this is aweful, we must refactor finisher system
        IEnumerator transformOvertime(Transform target)
        {
            float time = .3f;
            while (time > 0)
            {
                time -= Time.deltaTime;
                transform.LookAt(target);
                transform.position = target.position + target.forward * 1f;

                yield return null;
            }
            yield return new WaitForSeconds(.2f);
            target.GetComponent<HealthSystem>().Kill();
            PerformFinisherSkill();
            inQuickFinisher = false;
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
                    transform.position = grabTarget.position + grabTarget.forward * 1.2f;

                    Vector3 rot = freeLookCam.transform.rotation.eulerAngles;
                    rot = new Vector3(rot.x, rot.y + 180, rot.z);
                    grabTarget.rotation = Quaternion.Euler(rot);
                }
            }
        }

        private void testInput()
        {
            if (FinisherInput.FinisherMeterCheat())
            {
                increaseFinisherMeter(config.MaxFinisherMeter);
            }
            if (FinisherInput.VolatilityCheat() && grabTarget)
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
            print(combatSystem.CurrentAttackType);
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

        #region Hit Character

        public void HitCharacter(HealthSystem targetHealthSystem, float soulBonus=0)
        {
            if (!combatSystem.Hit.Add(targetHealthSystem))
            {
                return;
            }

            StabbedEnemy(targetHealthSystem.gameObject);
            if(combatSystem.CurrentAttackType == AttackType.LightBlade)
            {
                lightFinisherAttackDamageSystem.HitCharacter(gameObject, targetHealthSystem,bonusDamage:soulBonus);
            }
            else if(combatSystem.CurrentAttackType == AttackType.HeavyBlade)
            {
                heavyFinisherAttackDamageSystem.HitCharacter(gameObject, targetHealthSystem,bonusDamage:soulBonus);
            }

            combatSystem.IncrementHitCounter();
        }

        #endregion

        #endregion

        #region Toggle Grab Delegate Method

        public void ToggleGrabOff()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.INVULNERABLE_SEQUENCE_TAG) &&
                !animator.IsInTransition(0))
            {
                OnGrabbingTargetToggled(false);
            }
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
            grabTarget.GetComponent<CharacterState>().Grabbed = true;
            isFinishing = false;
            if (currentFinisherMeter - grabStun.FinisherMeterCost > 0)
            {
                decreaseFinisherMeter(grabStun.FinisherMeterCost);
            }
            Instantiate(grabStun, grabTarget.position, grabTarget.rotation);
            animator.SetBool(AnimConstants.Parameters.GRABBING_BOOL, true);
        }

        private void stopGrab()
        {
            if (grabTarget)
            {
                grabTarget.GetComponent<CharacterState>().Grabbed = false;
            }
            grabTarget = null;
            freeLookCam.NewFollowTarget = null;
            characterState.Grabbing = false;
            animator.SetBool(AnimConstants.Parameters.GRABBING_BOOL, false);
        }

        #endregion

        bool finisherModeActive = false;

        // subscribed to the OnFinisherModeToggled delegate
        private void toggleFinisherMode(bool enabled)
        {
            finisherModeActive = enabled;

            animator.SetBool(AnimConstants.Parameters.FINISHERMODE_BOOL, finisherModeActive);
            animator.SetTrigger(AnimConstants.Parameters.RESETPEACEFULLY_TRIGGER);

            if (enabled)
            {
                toggleWeapon(WeaponToggle.Knife);
                inFinisherIndicator.gameObject.SetActive(false);
                animOverrideSetter.SetTriggerOverride(AnimConstants.Parameters.BASIC_ACTION_TRIGGER, AnimConstants.OverrideIndexes.DEFAULT_BASIC_ACTION_INDEX, config.EnterFinisherModeAnim);
            }
            else
            {
                toggleWeapon(WeaponToggle.Sword);
                animOverrideSetter.SetTriggerOverride(AnimConstants.Parameters.BASIC_ACTION_TRIGGER, AnimConstants.OverrideIndexes.DEFAULT_BASIC_ACTION_INDEX, config.ExitFinisherModeAnim);
            }

            if (!finisherModeActive)
            {
                ToggleGrabOff();
            }

            updateFinisherMeterUI();
        }

        private void toggleWeapon(WeaponToggle weaponToggle)
        {
            knife.gameObject.SetActive(false);
            sword.gameObject.SetActive(false);
            armblade.gameObject.SetActive(false);

            switch (weaponToggle)
            {
                case WeaponToggle.Sword:
                    sword.gameObject.SetActive(true);
                    break;
                case WeaponToggle.Knife:
                    armblade.gameObject.SetActive(true);
                    knife.gameObject.SetActive(true);
                    break;

            }
        }

        private void updateFinisherMeterUI()
        {
            if (finisherMeter)
            {
                finisherMeter.SetFillAmount(GetFinisherMeterAsPercent());

                inFinisherIndicator.gameObject.SetActive(GetFinisherMeterAsPercent() >= 1 - Mathf.Epsilon && !finisherModeActive);
            }
        }

        #region Animation Events 

        public void GrabSuccessful()
        {
            OnGrabbingTargetToggled(true);
        }

        void FinisherExecutionSlice()
        {
            try
            {
                lightFinisherAttackDamageSystem.HitCharacter(gameObject, grabTarget.GetComponent<HealthSystem>());
                grabTarget.GetComponent<HealthSystem>().CutInHalf();
                if (finishTutorial != null)
                {
                    finishTutorial.showFinisherTutorial();
                }
            }
            catch(Exception ex)
            {
                
            }
            toggleWeapon(WeaponToggle.Knife);
        }

        void PerformFinisherSkill()
        {
            decreaseFinisherMeter(currentFinisherExecution.FinisherMeterCost);
            Instantiate(currentFinisherExecution, transform.position, transform.rotation);
            isFinishing = false;
        }

        void SoulInfusion()
        {
            isFinishing = false;
            if (soulTimer != null)
            {
                StopCoroutine(soulTimer);
            }
            soulOn();
            soulTimer = stopSoul(soulSwordTime);
            StartCoroutine(soulTimer);
        }

        private IEnumerator soulTimer;
        private IEnumerator stopSoul(float time) {
            yield return new WaitForSeconds(time);
            soulOff();

        }
        #endregion
        
    }
}
