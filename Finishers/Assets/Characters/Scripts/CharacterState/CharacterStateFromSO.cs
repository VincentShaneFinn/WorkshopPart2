using UnityEngine;

namespace Finisher.Characters
{
    public class CharacterStateFromSO : CharacterState
    {
        [SerializeField] public CharacterStateSO stateSO;

        void Start()
        {
            stateSO.State = this;
        }

    }
}