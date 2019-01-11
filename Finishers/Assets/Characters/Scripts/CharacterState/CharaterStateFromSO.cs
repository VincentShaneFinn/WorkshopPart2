using UnityEngine;

namespace Finisher.Characters
{
    public class CharaterStateFromSO : CharacterState
    {
        [SerializeField] private CharacterStateSO stateSO;

        //protected override void initialize()
        //{
        //    stateSO.Initialize(Animator); 
        //}

        public override DyingState DyingBool
        {
            get { return stateSO.DyingBool; }
            set { stateSO.DyingBool = value; }
        }
        public override bool Grabbing { get { return stateSO.Grabbing; } set { stateSO.Grabbing = value; } } 

    }
}