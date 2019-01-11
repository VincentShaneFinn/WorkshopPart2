using UnityEngine;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = "Finisher/States/CharacterState")]
    public class CharacterStateSO : ScriptableObject
    {

        // todo create property drawer to allow preview of setting dying, so that the delegate event gets called
        [SerializeField] public DyingState DyingBool;
        [SerializeField] public bool Grabbing;

    }
}
