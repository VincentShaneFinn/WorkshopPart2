using UnityEngine;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = ("Finisher/HealthSystem"))]
    public class HealthSystemConfig : ScriptableObject
    {
        [SerializeField] float maxHealth = 100f; public float MaxHealth { get { return maxHealth; } }
        [SerializeField] AnimationClip[] knockbackAnimations = null; public AnimationClip[] KnockbackAnimations { get { return knockbackAnimations; } }
    }
}
