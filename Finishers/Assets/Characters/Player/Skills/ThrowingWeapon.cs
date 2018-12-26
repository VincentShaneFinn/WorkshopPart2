using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Skills {
    public class ThrowingWeapon : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 75f;
        [SerializeField] private AnimationClip StandingDeathAnimClip;
        [Tooltip("This helps make sure the throwable can have a wide hitbox, but prevent it looking like you missed when they hand on the wall")]
        [SerializeField] private float xClamp = .2f;
        private bool hitFirstEnemy = false;
        private bool beginSpecialAttack = false;
        private Transform myEnemy;
        private Vector3 savedEnemyPosition;

        private BoxCollider boxCollider;
        private Rigidbody rigidBody;

        void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;

            rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        void Update()
        {
            if (beginSpecialAttack)
            {
                rigidBody.velocity = transform.forward * moveSpeed;
                if (myEnemy)
                {
                    myEnemy.localPosition = savedEnemyPosition;
                }
            }
        }

        public void ThrowWeapon()
        {
            beginSpecialAttack = true;
            boxCollider.enabled = true;
            transform.parent = null;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Enemy") // hit an enemy
            {
                Physics.IgnoreCollision(collision.collider, boxCollider);
                var healthSystem = collision.gameObject.GetComponent<HealthSystem>();
                if (healthSystem)
                {
                    if (!hitFirstEnemy)
                    {
                        KillEnemyAndMakeChild(collision, healthSystem);
                        hitFirstEnemy = true;
                    }
                    else
                    {
                        healthSystem.Damage(10);
                    }

                }
            }
            else // hit a wall
            {
                StopProjectileMovement();
            }
        }

        private void KillEnemyAndMakeChild(Collision collision, HealthSystem healthSystem)
        {
            collision.transform.parent = transform;
            healthSystem.Kill(StandingDeathAnimClip);
            myEnemy = collision.transform;
            savedEnemyPosition = collision.transform.localPosition;
            savedEnemyPosition.z = .5f;
            savedEnemyPosition.x = Mathf.Clamp(savedEnemyPosition.x, -xClamp, xClamp); // allow for 
        }

        private void StopProjectileMovement()
        {
            moveSpeed = 0;
            transform.position = transform.position;
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        }

        // todo make these despawn after a while
    }
}