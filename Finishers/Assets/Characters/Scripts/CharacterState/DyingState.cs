using System;
using UnityEngine;

namespace Finisher.Characters
{
    [Serializable]
    public class DyingState
    {
        private Animator animator;

        #region Delegates

        public event CharacterIsDying OnCharacterKilled;

        #endregion

        public DyingState(Animator anim)
        {
            animator = anim;
        }

        private bool dying = false;
        public bool Dying
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
                            animator.SetBool(AnimConstants.Parameters.DYING_BOOL, true);
                            dying = true;
                            OnCharacterKilled();
                        }
                    }
                }
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
