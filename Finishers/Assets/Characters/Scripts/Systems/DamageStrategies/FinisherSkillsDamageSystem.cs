using UnityEngine;

namespace Finisher.Characters.Systems.Strategies
{
    public class FinisherSkillsDamageSystem : FinisherModeDamageSystem
    {
        [SerializeField] private float finisherMeterCost = 10f; public float FinisherMeterCost { get { return finisherMeterCost; } }
    }
}