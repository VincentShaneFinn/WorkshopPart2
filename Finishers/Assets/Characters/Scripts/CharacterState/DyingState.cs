using System;
using UnityEngine;

namespace Finisher.Characters
{
    [Serializable]
    public class DyingState
    {

        #region Delegates

        public event CharacterIsDying OnCharacterKilled;

        #endregion

        public DyingState() { }

        private bool dying = false;
        public bool Dying
        {
            get { return dying; }
        }
        
        public void Kill()
        {
            dying = true;
            if (OnCharacterKilled != null)
            {
                OnCharacterKilled();
            }
        }

        public void SubscribeToDeathEvent(CharacterIsDying method)
        {
            OnCharacterKilled += method;
        }
        public void UnsubscribeToDeathEvent(CharacterIsDying method)
        {
            OnCharacterKilled -= method;
        }

    }
}
