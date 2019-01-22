using UnityEngine;

namespace Finisher.Characters
{
    public class ParrySMB : StateMachineBehaviour
    {
        public delegate void ParryExited();
        public event ParryExited ParryExitListeners;

        void OnStateExit()
        {
            if (ParryExitListeners != null)
            {
                ParryExitListeners();
            }
        }
    }
}
