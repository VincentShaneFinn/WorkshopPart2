using UnityEngine;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/HealthSystem"))]
    public class HealthSystemConfig : ScriptableObject
    {
        [SerializeField] AnimationClip[] normalDeathAnimations = null; public AnimationClip[] NormalDeathAnimations { get { return normalDeathAnimations; } }
        [SerializeField] AnimationClip[] knockbackAnimations = null; public AnimationClip[] KnockbackAnimations { get { return knockbackAnimations; } }
    }
}
