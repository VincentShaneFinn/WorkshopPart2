using UnityEngine;

namespace Finisher.Characters.Systems
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/FinisherConfig"))]
    public class FinisherConfig : ScriptableObject
    {
        [SerializeField] private float maxFinisherMeter = 100f; public float MaxFinisherMeter { get { return maxFinisherMeter; } }
        [SerializeField] private AnimationClip enterFinisherModeAnim; public AnimationClip EnterFinisherModeAnim { get { return enterFinisherModeAnim; } }
        [SerializeField] private AnimationClip exitFinisherModeAnim; public AnimationClip ExitFinisherModeAnim { get { return exitFinisherModeAnim; } }
    }
}