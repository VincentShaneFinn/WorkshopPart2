using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Finisher.Characters
{
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

        public void Dodge()
        {
            characterAnim.Dodge(config.DodgeAnimation);
        }

        public void LightAttack()
        {
            characterAnim.Attack(config.LightAttackAnimations[nextAttackIndex]);
            print(nextAttackIndex);
            nextAttackIndex++;
            nextAttackIndex %= config.LightAttackAnimations.Length;
        }

    }
}