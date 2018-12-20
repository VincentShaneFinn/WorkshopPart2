using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Finisher.Characters
{
    [RequireComponent(typeof(CharacterAnimator))]
    public class HealthSystem : MonoBehaviour
    {

        [SerializeField] HealthSystemConfig config;
        private CharacterAnimator character;

        void Start()
        {
            character = GetComponent<CharacterAnimator>();
            Assert.IsNotNull(config);
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.K))
            {
                character.Knockback(config.KnockbackAnimations[0]);
            }
        }
    }
}
