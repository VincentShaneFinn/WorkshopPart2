using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/CombatSystem"))]
    public class CombatSystemConfig : ScriptableObject
    {
        [SerializeField] AnimationClip dodgeForwardAnimation; public AnimationClip DodgeForwardAnimation { get { return dodgeForwardAnimation; } private set { } }
        [SerializeField] AnimationClip dodgeRightAnimation; public AnimationClip DodgeRightAnimation { get { return dodgeRightAnimation; } private set { } }
        [SerializeField] AnimationClip dodgeBackwardAnimation; public AnimationClip DodgeBackwardAnimation { get { return dodgeBackwardAnimation; } private set { } }
        [SerializeField] AnimationClip dodgeLeftAnimation; public AnimationClip DodgeLeftAnimation { get { return dodgeLeftAnimation; } private set { } }
        [SerializeField] float lightAttackDamage = 10f; public float LightAttackDamage { get { return lightAttackDamage; } private set { } }
        [SerializeField] float heavyAttackDamage = 20f; public float HeavyAttackDamage { get { return heavyAttackDamage; } private set { } }
    }
}