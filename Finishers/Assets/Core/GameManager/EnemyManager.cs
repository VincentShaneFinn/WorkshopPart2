using Finisher.Characters.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Core {
    public class EnemyManager : MonoBehaviour
    {

        public delegate void PlayerInCombatDelegate(bool inCombat);
        public event PlayerInCombatDelegate OnPlayerInCombatChanged;
        public void CallListeners(bool inCombat)
        {
            if (OnPlayerInCombatChanged != null)
            {
                OnPlayerInCombatChanged(inCombat);
            }
        }

        List<SquadManager> squadsInCombat = new List<SquadManager>();

        public void AddCombatSquad(SquadManager squad)
        {
            if(squadsInCombat.Contains(squad)) { return; }
            if (squadsInCombat.Count == 0)
            {
                CallListeners(true);
            }
            squadsInCombat.Add(squad);
        }

        public void RemoveCombatSquad(SquadManager squad)
        {
            if (squadsInCombat.Count == 0) { return; }

            if (squadsInCombat.Contains(squad))
            {
                squadsInCombat.Remove(squad);

                if (squadsInCombat.Count == 0)
                {
                    CallListeners(false);
                }
            }
        }

    }
}