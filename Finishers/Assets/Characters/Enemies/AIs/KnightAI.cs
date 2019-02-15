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

        protected override void Update()
        {
            if (knightCombatSystem.IsPerformingSpecialAttack)
            {
                StopCurrentCoroutine();
                return;
            }

            base.Update();

            if (Input.GetKeyDown(KeyCode.T))
            {
                //Invoke("test", Random.Range(3,5));
                test();
            }
        }

        private void test()
        {
            knightCombatSystem.RushAttack(combatTarget.transform);
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
