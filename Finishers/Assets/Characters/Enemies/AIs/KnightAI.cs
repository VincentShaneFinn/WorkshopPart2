using System.Collections;
using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Enemies
{
    public class KnightAI : EnemyAI
    {
        private bool useRushAttack = false;
        private bool tempinvokedSetup = false;

        KnightCombatSystem knightCombatSystem;

        protected override void Start()
        {
            base.Start();

            knightCombatSystem = GetComponent<KnightCombatSystem>();
        }

        private void setContext()
        {
            useRushAttack = true;
            tempinvokedSetup = false;
        }

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                knightCombatSystem.RushAttack(combatTarget.transform);
            }
        }

        protected override void attackPlayer()
        {
            if (useRushAttack)
            {
                useRushAttack = false;
                knightCombatSystem.RushAttack(combatTarget.transform);
            }
            else
            {
                base.attackPlayer();
            }
        }

    }
}
