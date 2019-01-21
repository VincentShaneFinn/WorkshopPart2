using UnityEngine;

namespace Finisher.Characters
{
    public class DodgeSMB : StateMachineBehaviour
    {
        public delegate void DodgeExited();
        public event DodgeExited DodgeExitListeners;

        void OnStateExit()
        {
            if (DodgeExitListeners != null)
            {
                DodgeExitListeners();
            }
        }
    }
}
