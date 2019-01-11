using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Finisher.Characters
{
    public class CharacterStateFromSelf : CharacterState
    {

        #region Delegates

        public event CharacterIsDying OnCharacterKilled;

        #endregion

        private bool dying = false;
        public override bool Dying
        {
            get { return dying; }
            set
            {
                if (!dying)
                {
                    if (value)
                    {
                        if (OnCharacterKilled != null)
                        {
                            Animator.SetBool(AnimConstants.Parameters.DYING_BOOL, true);
                            dying = true;
                            OnCharacterKilled();
                        }
                    }
                }
            }
        }
        public override void SubscribeToDeathEvent(CharacterIsDying method)
        {
            OnCharacterKilled += method;
        }
        public override void UnsubscribeToDeathEvent(CharacterIsDying method)
        {
            OnCharacterKilled -= method;
        }

    }
}