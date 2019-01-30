using UnityEngine;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = "Finisher/States/CharacterState")]
    public class CharacterStateSO : ScriptableObject
    {
        public CharacterState state;

        // todo create property drawer to allow preview of setting dying, so that the delegate event gets called
        [SerializeField] public DyingState DyingState;
        [SerializeField] public bool Grabbing;

    }
}
