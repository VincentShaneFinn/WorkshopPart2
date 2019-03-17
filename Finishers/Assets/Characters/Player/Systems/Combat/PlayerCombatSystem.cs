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
        [SerializeField] float impactFrameTime = .01f;

        private PlayerCharacterController playerCharacter; // A reference to the ThirdPersonCharacter on the object

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

            if (hitCounter > 0)
            {
                timer += Time.deltaTime;

                if (timer >= config.TimeToKeepCombo)
                {
                    resetHitCounter();
                    timer = 0;
                }
            }
        }

        private void processCombatInput()
        {
            processAttackInput();
            processDodgeInput();
            processParryInput();
        }

        private void processAttackInput()
        {
            if (FinisherInput.LightAttack())
            {
                LightAttack();
            }

            if (FinisherInput.HeavyAttack())
            {
                HeavyAttack();
            }
        }

        private void processDodgeInput()
        {
            if (FinisherInput.Dodge())
            {
                finisherSystem.ToggleGrabOff();
                var dodgeDirection = GetMoveDirection();
                Dodge(dodgeDirection);
            }
        }

        private void processParryInput()
        {
            if (FinisherInput.Parry())
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

        public override void HitCharacter(HealthSystem target, float soulBonus = 0)
        {
            base.HitCharacter(target,soulBonus);
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
                StartCoroutine(killOnStab(enemyToParry)); //TODO: MUST REPLACE THIS WITH A BETTER WAY
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

        IEnumerator killOnStab(HealthSystem enemyToParry)
        {
            yield return new WaitForSeconds(.75f);
            lightAttackDamageSystem.HitCharacter(gameObject, enemyToParry, bonusDamage: 10);
            CallCameraShakeEvent(0.5f, heavyAttackDamageSystem.KnockbackDuration * 1.5f);

            if (enemyToParry.GetHealthAsPercent() <= 0)
            {
                enemyToParry.Kill(config.RiposteKillAnimationToPass, overrideKillAnim: true);
            }
        }

        //TODO: create a linked character animation system
        IEnumerator transformOvertime(Transform target)
        {
            float time = .3f;
            while(time > 0)
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