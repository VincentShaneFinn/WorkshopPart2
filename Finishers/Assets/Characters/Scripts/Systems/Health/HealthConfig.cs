using UnityEngine;

namespace Finisher.Characters.Systems
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/HealthConfig"))]
    public class HealthConfig : ScriptableObject
    {
        [SerializeField] float maxHealth = 100f; public float MaxHealth { get { return maxHealth; } }
        [SerializeField] float maxVolatility = 100f; public float MaxVolatility { get { return maxVolatility; } }
        [SerializeField] int knockbackLimit = 2; public int KnockbackLimit { get { return knockbackLimit; } }
        [SerializeField] float freeKnockbackTime = 1f; public float FreeKnockbackTime { get { return freeKnockbackTime; } }

        [SerializeField] AnimationClip[] normalDeathAnimations = null; public AnimationClip[] NormalDeathAnimations { get { return normalDeathAnimations; } }
        [SerializeField] AnimationClip[] knockbackAnimations = null; public AnimationClip[] KnockbackAnimations { get { return knockbackAnimations; } }
    }
}
