using UnityEngine;

using Finisher.Characters.Systems;

public abstract class FinisherExecution : MonoBehaviour
{
    public abstract float FinisherMeterCost { get; }
    public abstract AnimationClip AnimationToPlay { get; }

    public CombatSystem combatSystem;
}
