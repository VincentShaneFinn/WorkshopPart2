using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Player.Finishers {
    public class ThrowingWeaponAOE : FlameAOE
    {
        public void setSource(HealthSystem sourceHealthSystem)
        {
            hit.Add(sourceHealthSystem);
        }
    }
}