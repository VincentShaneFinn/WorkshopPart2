using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Player.Finishers
{
    public class Blade : MonoBehaviour
    {
        //[System.NonSerialized]
        public FinisherSkillsDamageSystem damageSystem;
        //[System.NonSerialized]
        public HealthSystem target;
        [SerializeField] public float speedrate=1;
        protected float speed=0;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            transform.Translate(0,0,speed,transform);
            speed += speedrate*Time.deltaTime;
            float d = (transform.position - (target.transform.position+Vector3.up)).magnitude;
            if (d<=.3f) {
                damageSystem.HitCharacter(gameObject,target);
                Destroy(gameObject);
            }
            if (d <= speed)
            {
                speed = d;
            }
            transform.LookAt(target.transform.position+Vector3.up);
        }
    }
}
