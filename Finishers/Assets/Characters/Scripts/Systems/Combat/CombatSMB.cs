using UnityEngine;

namespace Finisher.Characters
{
    public class CombatSMB : StateMachineBehaviour
    {
        public delegate void AttackExited();
        public event AttackExited AttackExitListeners;

        void OnStateExit()
        {
            if (AttackExitListeners != null)
            {
                AttackExitListeners();
            }
        }
    }
}
