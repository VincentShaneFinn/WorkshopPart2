using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Finisher.Characters.Systems.Strategies
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/Particle/ParticleEvent"))]
    public class ParticleEvent : ScriptableObject
    {
        [SerializeField] private GameObject particleObject;

        public void Play(Vector3 pos, Quaternion rot)
        {
            ParticleSystem[] particlesystems = particleObject.GetComponentsInChildren<ParticleSystem>();

            if (particlesystems.Length > 0)
            {
                //TODO: try to put this somewhere so it doesnt flood the hierarchy
                GameObject particleSystemContainer = new GameObject();
                float longestDuration = 0;
                for (int i = 0; i < particlesystems.Length; i++)
                {
                    ParticleSystem particleSystem = Instantiate(particlesystems[i], pos,rot);
                    particleSystem.transform.parent = particleSystemContainer.transform;

                    particleSystem.Play();
                    if (particleSystem.main.duration > longestDuration)
                    {
                        longestDuration = particleSystem.main.duration;
                    }
                }

                Destroy(particleSystemContainer, longestDuration);
            }
        }

    }
}