using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = ("Finisher/CombatSystem"))]
    public class CombatSystemConfig : ScriptableObject
    {
        [SerializeField] AnimationClip dodgeAnimation; public AnimationClip DodgeAnimation { get { return dodgeAnimation; } private set { } }
        [SerializeField] float lightAttackDamage;
        [Tooltip("Put up to 4 attack animations here")]
        [SerializeField] AnimationClip[] lightAttackAnimations = null; public AnimationClip[] LightAttackAnimations { get { return lightAttackAnimations; } private set { } }
    }
}