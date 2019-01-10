using UnityEngine;

namespace Finisher.Characters.Systems
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/HealthSystem"))]
    public class HealthConfig : ScriptableObject
    {
        [SerializeField] AnimationClip[] normalDeathAnimations = null; public AnimationClip[] NormalDeathAnimations { get { return normalDeathAnimations; } }
        [SerializeField] AnimationClip[] knockbackAnimations = null; public AnimationClip[] KnockbackAnimations { get { return knockbackAnimations; } }
    }
}
