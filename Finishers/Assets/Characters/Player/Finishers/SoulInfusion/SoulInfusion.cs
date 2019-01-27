using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Player.Finishers {
    public class SoulInfusion : FinisherExecution
    {
        [SerializeField] private float finisherMeterCost = 50f; public override float FinisherMeterCost { get { return finisherMeterCost; } }
        [SerializeField] private AnimationClip animationToPlay; public override AnimationClip AnimationToPlay { get { return animationToPlay; } }

        // todo make configs for Finisher

        void Start()
        {
            Destroy(gameObject, animationToPlay.length);
        }


    }
}