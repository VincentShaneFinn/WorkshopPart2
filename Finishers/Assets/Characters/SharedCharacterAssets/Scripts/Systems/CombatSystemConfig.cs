using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/CombatSystem"))]
    public class CombatSystemConfig : ScriptableObject
    {
        [SerializeField] AnimationClip dodgeForwardAnimation; public AnimationClip DodgeForwardAnimation { get { return dodgeForwardAnimation; } }
        [SerializeField] AnimationClip dodgeRightAnimation; public AnimationClip DodgeRightAnimation { get { return dodgeRightAnimation; } }
        [SerializeField] AnimationClip dodgeBackwardAnimation; public AnimationClip DodgeBackwardAnimation { get { return dodgeBackwardAnimation; } }
        [SerializeField] AnimationClip dodgeLeftAnimation; public AnimationClip DodgeLeftAnimation { get { return dodgeLeftAnimation; } }
        [SerializeField] float lightAttackDamage = 10f; public float LightAttackDamage { get { return lightAttackDamage; } }
        [SerializeField] float heavyAttackDamage = 20f; public float HeavyAttackDamage { get { return heavyAttackDamage; } }
    }
}