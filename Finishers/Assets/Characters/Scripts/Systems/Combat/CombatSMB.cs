using UnityEngine;

namespace Finisher.Characters
{
    public class CombatSMB : StateMachineBehaviour
    {
        public delegate void AttackExited();
        public event AttackExited AttackExitListeners;

        public delegate void AttackStarted();
        public event AttackStarted AttackStartListeners;

        void OnStateEnter()
        {
            if (AttackStartListeners != null)
            {
                AttackStartListeners();
            }
        }

        void OnStateExit()
        {
            if (AttackExitListeners != null)
            {
                AttackExitListeners();
            }
        }
    }
}
