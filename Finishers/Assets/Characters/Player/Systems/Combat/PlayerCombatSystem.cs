using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Finisher.Core;
using Finisher.Characters.Systems;
using System;

namespace Finisher.Characters.Player.Systems
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerCharacterController))]
    [RequireComponent(typeof(FinisherSystem))]
    public class PlayerCombatSystem : CombatSystem
    {

        [Tooltip("This is a timer that puts a freeze time on both you and the target you hit")]
        [SerializeField] float impactFrameTime = .1f;

        private PlayerCharacterController playerCharacter; // A reference to the ThirdPersonCharacter on the object
        private float chargeTime;
        private float chargeAttackDelay = 0.1f;
        private bool lightattackInitiated = false;
        private bool heavyattackInitiated = false;

        protected override void Start()
        {
            base.Start();

            // get the third person character ( this should never be null due to require component )
            playerCharacter = GetComponent<PlayerCharacterController>();
            finisherSystem = GetComponent<FinisherSystem>();
            startFixedDeltaTime = Time.fixedDeltaTime;
        }

        private void Update()
        {
            if (GameManager.instance.GamePaused) { return; }

            if (playerCharacter.isGrounded)
            {
                processCombatInput();
            }

            testingInputZone();
        }

        private void processCombatInput()
        {
            processAttackInput();
            processDodgeInput();
        }

        private void processAttackInput()
        {
            if (Input.GetButtonDown(InputNames.LightAttack))
            {
                chargeTime = Time.time;
                lightattackInitiated = true;
            }
            if (Input.GetButtonUp(InputNames.LightAttack) && Time.time <= chargeTime + chargeAttackDelay && lightattackInitiated)
            {
                LightAttack();
                lightattackInitiated = false;
            }
            else if (Time.time > chargeTime + chargeAttackDelay && lightattackInitiated)
            {
                ChargeLightAttack();
                lightattackInitiated = false;
            }
            if (ControlMethodDetector.GetCurrentControlType() == ControlType.Xbox)
            {
                if (Input.GetAxisRaw(InputNames.HeavyAttack) > 0) // xbox triggers are not buttons
                {
                    chargeTime = Time.time;
                    heavyattackInitiated = true;
                }
                if (Input.GetAxisRaw(InputNames.HeavyAttack) <= 0 && Time.time <= chargeTime + chargeAttackDelay && heavyattackInitiated)
                {
                    HeavyAttack();
                    heavyattackInitiated = false;
                }
                else if (Input.GetAxisRaw(InputNames.HeavyAttack) <= 0 && Time.time > chargeTime + chargeAttackDelay && heavyattackInitiated)
                {
                    ChargeHeavyAttack();
                    heavyattackInitiated = false;
                }
            }
            else
            {
                if (Input.GetButtonDown(InputNames.HeavyAttack))
                {
                    chargeTime = Time.time;
                    heavyattackInitiated = true;
                }
                if (Input.GetButtonUp(InputNames.HeavyAttack) && Time.time <= chargeTime + chargeAttackDelay && heavyattackInitiated)
                {
                    HeavyAttack();
                    heavyattackInitiated = false;
                }
                else if (Time.time > chargeTime + chargeAttackDelay && heavyattackInitiated)
                {
                    ChargeHeavyAttack();
                    heavyattackInitiated = false;
                }
            }
        }

        private void processDodgeInput()
        {
            if (Input.GetButtonDown(InputNames.Dodge) || Input.GetKeyDown(KeyCode.Mouse3))
            {
                finisherSystem.ToggleGrabOff();
                var dodgeDirection = GetMoveDirection();
                Dodge(dodgeDirection);
            }
            if (Input.GetButtonDown(InputNames.Parry) || Input.GetKeyDown(KeyCode.Mouse4))
            {
                finisherSystem.ToggleGrabOff();
                Parry();
            }
        }

        private MoveDirection GetMoveDirection()
        {
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(vertical) >= Mathf.Abs(horizontal)) // forward backward carries more weight
            {
                if (vertical >= 0)
                {
                    return MoveDirection.Forward;
                }
                else
                {
                    return MoveDirection.Backward;
                }
            }
            else
            {
                if (horizontal >= 0)
                {
                    return MoveDirection.Right;
                }
                else
                {
                    return MoveDirection.Left;
                }
            }
        }

        private float startFixedDeltaTime;
        private void testingInputZone()
        {
            // todo remove this testing code
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Time.timeScale = .1f;
                Time.fixedDeltaTime = startFixedDeltaTime * Time.timeScale;
            }
            if (Input.GetKeyUp(KeyCode.Z))
            {
                Time.timeScale = 1;
                Time.fixedDeltaTime = startFixedDeltaTime;
            }
        }

        public override void HitCharacter(HealthSystem target)
        {
            base.HitCharacter(target);
            StartCoroutine(ImpactFrames(target));
        }

        IEnumerator ImpactFrames(HealthSystem targetHealthSystem)
        {
            animator.speed = 0;
            targetHealthSystem.GetComponent<Animator>().speed = 0;

            yield return new WaitForSeconds(impactFrameTime);

            animator.GetComponent<Animator>().speed = 1;
            targetHealthSystem.GetComponent<Animator>().speed = 1;
        }

        protected override void attemptRiposte()
        {
            var enemies = getEnemiesInFront();
            var enemyToParry = getEnemyToParry(enemies);
            if (enemyToParry)
            {
                characterState.EnterInvulnerableActionState(config.RiposteAnimation);
                StartCoroutine(transformOvertime(enemyToParry.transform));
                enemyToParry.Kill(config.RiposteKillAnimationToPass);
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

        private HealthSystem getEnemyToParry(List<Collider> enemies)
        {
            foreach (var enemy in enemies)
            {
                CharacterState enemyState = enemy.GetComponent<CharacterState>();
                if (enemyState && enemyState.Parried)
                {
                    return enemy.GetComponent<HealthSystem>();
                }
            }

            return null;
        }

        //TODO: create a linked character animation system
        IEnumerator transformOvertime(Transform target)
        {
            float time = .3f;
            while (time > 0)
            {
                time -= Time.deltaTime;
                transform.LookAt(target);
                transform.position = target.position + target.forward;

                yield return null;
            }
            yield return new WaitForSeconds(.5f); //TODO: this should be called by an animation event on the parry animation
            CallCombatSystemDealtDamageListeners(10f); //TODO: REMOVE MAGIC NUMBER AND PUT IN CONFIG
        }

    }
}