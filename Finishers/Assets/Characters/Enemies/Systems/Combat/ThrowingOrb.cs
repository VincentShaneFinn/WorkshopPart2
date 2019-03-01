using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Player.Finishers {
    public class ThrowingOrb : MonoBehaviour
    {
        [SerializeField] DamageSystem orbDamageSystem;
        [SerializeField] private float moveSpeed = 75f;

        private Rigidbody rigidBody;

        void Start()
        {

            rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

            Destroy(gameObject, 10f);
        }

        void Update()
        {
            rigidBody.velocity = transform.forward * moveSpeed;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player") // hit an enemy
            {
                var targetHealthSystem = collision.gameObject.GetComponent<HealthSystem>();
                if (targetHealthSystem)
                {
                    orbDamageSystem.HitCharacter(gameObject, targetHealthSystem);
                }
            }
            if (collision.gameObject.tag != "Enemy")
            {
                Destroy(gameObject);
            }
        }

        // todo make these despawn after a while
    }
}