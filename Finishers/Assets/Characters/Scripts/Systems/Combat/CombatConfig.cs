using UnityEngine;

namespace Finisher.Characters.Systems
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/CombatConfig"))]
    public class CombatConfig : ScriptableObject
    {
        [SerializeField] AnimationClip dodgeForwardAnimation; public AnimationClip DodgeForwardAnimation { get { return dodgeForwardAnimation; } }
        [SerializeField] AnimationClip dodgeRightAnimation; public AnimationClip DodgeRightAnimation { get { return dodgeRightAnimation; } }
        [SerializeField] AnimationClip dodgeBackwardAnimation; public AnimationClip DodgeBackwardAnimation { get { return dodgeBackwardAnimation; } }
        [SerializeField] AnimationClip dodgeLeftAnimation; public AnimationClip DodgeLeftAnimation { get { return dodgeLeftAnimation; } }
        [SerializeField] private float timeToClearAttackTrigger = 0; public float TimeToClearAttackTrigger { get { return timeToClearAttackTrigger; } }
        [SerializeField] private float attackAnimSpeed = 1f; public float AttackAnimSpeed { get { return attackAnimSpeed; } }
        [SerializeField] private float lightKnockback = 0.5f; public float LightKnockback {  get { return lightKnockback;  } }
        [SerializeField] private float heavyKnockback = 1f; public float HeavyKnockback { get { return heavyKnockback; } }
        [SerializeField] private float knockbackDuration = 0.1f; public float KnockbackDuration { get { return knockbackDuration; } }
    }
}