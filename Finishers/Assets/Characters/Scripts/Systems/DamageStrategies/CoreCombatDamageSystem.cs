using UnityEngine;

namespace Finisher.Characters.Systems.Strategies
{
    public class CoreCombatDamageSystem : DamageSystem
    {
        [SerializeField] private float finisherMeterGainAmount = 10f; public float FinisherMeterGainAmount { get { return finisherMeterGainAmount; } }
    }
}