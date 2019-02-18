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
        }

        public void PerformRushAttack()
        {
            knightCombatSystem.RushAttack(combatTarget.transform);
        }

    }
}
