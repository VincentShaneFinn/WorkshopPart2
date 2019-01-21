using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Finisher.Characters.Systems.Strategies
{
    [CreateAssetMenu(menuName = ("Finisher/Systems/Particle/ParticleEvent"))]
    public class ParticleEvent : ScriptableObject
    {
        [SerializeField] private GameObject particleobjects;
        public ParticleSystem[] particlesystems=null;

        public void play(Transform t) {
            if (particlesystems == null)
            {
                particlesystems = particleobjects.GetComponentsInChildren<ParticleSystem>();
            }
            if (particlesystems.Length > 0)
            {
                
                for (int i=0;i<particlesystems.Length;i++) {
                    Debug.Log("effect");
                    ParticleSystem n = Instantiate(particlesystems[i], t);
                    n.Play();
                    Destroy(n, particlesystems[i].main.duration);
                }
            }
            
        }
        public void play(Vector3 pos,Quaternion rot)
        {   
            //if (particlesystems == null)
            //{
                particlesystems = particleobjects.GetComponentsInChildren<ParticleSystem>();
                Debug.Log(particlesystems.Length);
            //}
            if (particlesystems.Length > 0)
            {
                for (int i = 0; i < particlesystems.Length; i++)
                {
                    Debug.Log("effect");
                    ParticleSystem n = Instantiate(particlesystems[i], pos,rot);
                    n.Play();
                    Destroy(n, particlesystems[i].main.duration);
                }
            }
            
        }
    }
}