using System.Collections;
using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters.Enemies
{
    public class KnightAI : EnemyAI
    {
        KnightCombatSystem knightCombatSystem;

        protected override void Start()
        {
            base.Start();

            knightCombatSystem = GetComponent<KnightCombatSystem>();
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
                PerformRushAttack();
            }
        }

        public void PerformRushAttack()
        {
            knightCombatSystem.RushAttack(combatTarget.transform);
        }

    }
}
