using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Systems.Strategies
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/Particle/ParticleEventSystem"))]
    public class ParticleEventSystem : ScriptableObject
    {
        [SerializeField] public ParticleEvent[] particleEvent;

        public void play(Vector3 pos, Quaternion rot)
        {
            if (particleEvent != null)
            {
                int i = ((int)UnityEngine.Random.Range(0, particleEvent.Length));
                particleEvent[i].Play(pos, rot);
            }
        }
    }
}
