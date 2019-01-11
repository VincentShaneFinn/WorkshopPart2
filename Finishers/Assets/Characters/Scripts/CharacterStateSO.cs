using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = "Finisher/States/CharacterState")]
    public class CharacterStateSO : ScriptableObject
    {

        #region Delegates

        public event CharacterIsDying OnCharacterKilled;

        #endregion

        // todo create property drawer to allow preview of setting dying, so that the delegate event gets called
        private bool dying = false;
        public bool Dying
        {
            get
            {
                return dying;
            }
            set
            {
                if (!dying)
                {
                    if (value)
                    {
                        dying = true;
                        if (OnCharacterKilled != null)
                        {
                            OnCharacterKilled();
                        }
                    }
                }
            }
        }

    }
}
