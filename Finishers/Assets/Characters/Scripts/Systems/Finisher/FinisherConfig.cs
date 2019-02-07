using UnityEngine;

namespace Finisher.Characters.Systems
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/FinisherConfig"))]
    public class FinisherConfig : ScriptableObject
    {
        [SerializeField] private float maxFinisherMeter = 100f; public float MaxFinisherMeter { get { return maxFinisherMeter; } }
    }
}