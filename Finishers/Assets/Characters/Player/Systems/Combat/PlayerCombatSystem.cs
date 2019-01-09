﻿using UnityEngine;

using Finisher.Core;

namespace Finisher.Characters.Systems
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerCharacterController))]
    [RequireComponent(typeof(FinisherSystem))]
    public class PlayerCombatSystem : CombatSystem
    {

        private PlayerCharacterController character; // A reference to the ThirdPersonCharacter on the object
        private FinisherSystem finisherSystem;

        protected override void Start()
        {
            base.Start();

            // get the third person character ( this should never be null due to require component )
            character = GetComponent<PlayerCharacterController>();
            finisherSystem = GetComponent<FinisherSystem>();
            startFixedDeltaTime = Time.fixedDeltaTime;
        }

        private void Update()
        {
            if (GameManager.instance.GamePaused) { return; }

            if (character.isGrounded)
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
                LightAttack();
            }
            if (ControlMethodDetector.GetCurrentControlType() == ControlType.Xbox)
            {
                if (Input.GetAxisRaw(InputNames.HeavyAttack) > 0) // xbox triggers are not buttons
                {
                    HeavyAttack();
                }
            }
            else
            {
                if (Input.GetButtonDown(InputNames.HeavyAttack))
                {
                    HeavyAttack();
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

    }
}