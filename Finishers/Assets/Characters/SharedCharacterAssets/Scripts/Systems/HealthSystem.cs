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
        private CharacterAnimator characterAnim;

        void Start()
        {
            characterAnim = GetComponent<CharacterAnimator>();
            Assert.IsNotNull(config);
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.K))
            {
                characterAnim.Knockback(config.KnockbackAnimations[UnityEngine.Random.Range(0,config.KnockbackAnimations.Length)]);
            }
        }
    }
}
