using UnityEngine;

namespace Finisher.Characters
{
    public class RangedAttackSMB : StateMachineBehaviour
    {

        public delegate void RangeStarted();
        public event RangeStarted RangeExitListeners;


        void OnStateExit()
        {
            if (RangeExitListeners != null)
            {
                RangeExitListeners();
            }
        }

    }
}
