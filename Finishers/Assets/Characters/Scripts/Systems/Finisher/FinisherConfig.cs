using UnityEngine;

namespace Finisher.Characters.Systems
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/FinisherConfig"))]
    public class FinisherConfig : ScriptableObject
    {
        [SerializeField] private float maxFinisherMeter = 100f; public float MaxFinisherMeter { get { return maxFinisherMeter; } }
        [SerializeField] private float lightFinisherGain = 10f; public float LightFinisherGain { get { return lightFinisherGain; } }
        [SerializeField] private float heavyFinisherGain = 30f; public float HeavyFinisherGain { get { return heavyFinisherGain; } }
        [SerializeField] private float lightVolatilityDamage = 10f; public float LightVolatilityDamage { get { return lightVolatilityDamage; } }
        [SerializeField] private float heavyVolatilityDamage = 30f; public float HeavyVolatilityDamage { get { return heavyVolatilityDamage; } }
    }
}