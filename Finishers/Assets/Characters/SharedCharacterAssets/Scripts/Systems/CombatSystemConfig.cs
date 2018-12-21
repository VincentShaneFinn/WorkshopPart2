using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = ("Finisher/CombatSystem"))]
    public class CombatSystemConfig : ScriptableObject
    {
        [SerializeField] AnimationClip dodgeForwardAnimation; public AnimationClip DodgeForwardAnimation { get { return dodgeForwardAnimation; } private set { } }
        [SerializeField] AnimationClip dodgeRightAnimation; public AnimationClip DodgeRightAnimation { get { return dodgeRightAnimation; } private set { } }
        [SerializeField] AnimationClip dodgeBackwardAnimation; public AnimationClip DodgeBackwardAnimation { get { return dodgeBackwardAnimation; } private set { } }
        [SerializeField] AnimationClip dodgeLeftAnimation; public AnimationClip DodgeLeftAnimation { get { return dodgeLeftAnimation; } private set { } }
        [SerializeField] float lightAttackDamage;
        [Tooltip("Put up to 4 attack animations here")]
        [SerializeField] AnimationClip[] lightAttackAnimations = null; public AnimationClip[] LightAttackAnimations { get { return lightAttackAnimations; } private set { } }
        [Tooltip("put the same amount of attack offsets, this is the time subtracted from the each clip length to allow the next action be played then if qued")]
        [SerializeField] float[] lightAttackOffsets = null; public float[] LightAttackOffsets { get { return lightAttackOffsets; } private set { } }
    }
}