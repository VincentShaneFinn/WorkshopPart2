using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Systems;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Player.Finishers {
    public class ThrowingWeapon : MonoBehaviour
    {
        [SerializeField] private ThrowingWeaponDamageSystem throwingWeaponDamageSystem;
        public float FinisherMeterCost { get { return throwingWeaponDamageSystem.FinisherMeterCost; } }
        [SerializeField] private float moveSpeed = 75f;
        [Tooltip("This helps make sure the throwable can have a wide hitbox, but prevent it looking like you missed when they hand on the wall")]
        [SerializeField] private float xClamp = .2f;
        private bool beginSpecialAttack = false;
        private List<Transform> myEnemies;
        private List<Vector3> savedEnemyPositions;

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

            myEnemies = new List<Transform>();
            savedEnemyPositions = new List<Vector3>();
        }

        void Update()
        {
            if (beginSpecialAttack)
            {
                rigidBody.velocity = transform.forward * moveSpeed;
                for (int i = 0; i < myEnemies.Count; i++)
                {
                    myEnemies[i].localPosition = savedEnemyPositions[i];
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
                var targetHealthSystem = collision.gameObject.GetComponent<HealthSystem>();
                if (targetHealthSystem)
                {
                    throwingWeaponDamageSystem.HitCharacter(gameObject, targetHealthSystem);
                    if (targetHealthSystem.GetComponent<CharacterState>().Dying)
                    {
                        MakeChild(collision);
                    }
                }
            }
            else // hit a wall
            {
                StopProjectileMovement();
            }
        }

        private void MakeChild(Collision targetCol)
        {
            targetCol.transform.parent = transform;
            myEnemies.Add(targetCol.transform);

            Vector3 pos = targetCol.transform.localPosition;
            pos.z = .5f;
            pos.x = Mathf.Clamp(pos.x, -xClamp, xClamp); // allow for 

            savedEnemyPositions.Add(pos);
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