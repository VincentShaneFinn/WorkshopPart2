using UnityEngine;

namespace Finisher.Characters
{
    public class CharaterStateFromSO : CharacterState
    {
        [SerializeField] private CharacterStateSO stateSO;

        public override DyingState DyingState
        {
            get { return stateSO.DyingState; }
            set { stateSO.DyingState = value; }
        }
        public override bool Grabbing { get { return stateSO.Grabbing; } set { stateSO.Grabbing = value; } } 

    }
}