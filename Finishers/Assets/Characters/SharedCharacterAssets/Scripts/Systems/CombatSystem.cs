using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Finisher.Characters
{
    public enum MoveDirection { Forward,Right,Backward,Left };

    // todo this and the AnimatorStatehandler need to talk to each other better, if this is going to override it, as simple as being a suggestor at certain times 
    public class CombatSystem : MonoBehaviour
    {
        [SerializeField] CombatSystemConfig config;

        //[HideInInspector] public bool PerformingAction = false;

        private CharacterAnimator characterAnim;

        private int nextAttackIndex = 0;

        // Start is called before the first frame update
        void Start()
        {
            characterAnim = GetComponent<CharacterAnimator>();
            //Assert.IsNotNull(config); // todo utilize the config appropriately
        }

        public void Dodge(MoveDirection moveDirection = MoveDirection.Forward)
        {
            AnimationClip animToUse;
            switch (moveDirection)
            {
                case MoveDirection.Right:
                    animToUse = config.DodgeRightAnimation;
                    break;
                case MoveDirection.Backward:
                    animToUse = config.DodgeBackwardAnimation;
                    break;
                case MoveDirection.Left:
                    animToUse = config.DodgeLeftAnimation;
                    break;
                default:
                    animToUse = config.DodgeForwardAnimation;
                    break;
            }
            if (characterAnim.CanRotate)
            {
                characterAnim.Dodge(config.DodgeForwardAnimation);
            }
            else
            {
                characterAnim.Dodge(animToUse);
            }
        }

        public void LightAttack()
        {
            StopAllCoroutines();
            characterAnim.LightAttack();
            //StartCoroutine(preventActionUntilJustBefore(config.LightAttackAnimations[nextAttackIndex].length - config.LightAttackOffsets[nextAttackIndex]));
        }

        public void HeavyAttack()
        {
            characterAnim.HeavyAttack();
        }

            IEnumerator preventActionUntilJustBefore(float time)
        {
            float count = 0;
            while(count < time && characterAnim.CanDodge)
            {
                count += Time.deltaTime;
                characterAnim.CanAct = false;
                yield return null;
            }
            characterAnim.CanAct = true;
        }

    }
}