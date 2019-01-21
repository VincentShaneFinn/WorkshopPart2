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
        [SerializeField] AnimationClip riposteAnimation; public AnimationClip RiposteAnimation { get { return riposteAnimation; } }
        [SerializeField] AnimationClip riposteKillAnimationToPass; public AnimationClip RiposteKillAnimationToPass { get { return riposteKillAnimationToPass; } }
        [SerializeField] private float timeToClearAttackTrigger = 0; public float TimeToClearAttackTrigger { get { return timeToClearAttackTrigger; } }
        [SerializeField] private float attackAnimSpeed = 1f; public float AttackAnimSpeed { get { return attackAnimSpeed; } }
    }
}