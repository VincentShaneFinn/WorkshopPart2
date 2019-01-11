using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Finisher.Characters
{
    public class CharaterStateFromSO : CharacterState
    {
        [SerializeField] private CharacterStateSO stateSO;

        public override bool Dying
        {
            get { return stateSO.Dying; }
            set { stateSO.Dying = value; }
        }
        public override void SubscribeToDeathEvent(CharacterIsDying method)
        {
            stateSO.OnCharacterKilled += method;
        }
        public override void UnsubscribeToDeathEvent(CharacterIsDying method)
        {
            stateSO.OnCharacterKilled -= method;
        }

    }
}