using UnityEngine;

public abstract class FinisherExecution : MonoBehaviour
{
    public abstract float FinisherMeterCost { get; }
    public abstract AnimationClip AnimationToPlay { get; }
}
