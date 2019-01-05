using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters.Finishers {
    public class FlameAOE : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float knockbackRange = 2f;
        [SerializeField] private float aoeRadius = 5f;
        [SerializeField] private float destroyInNSeconds = 1f;

        private List<Transform> myEnemies;

        private CapsuleCollider capsuleCollider;

        void Awake()
        {
            //capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            //capsuleCollider.enabled = true;
            //capsuleCollider.isTrigger = true;

            myEnemies = new List<Transform>(); // todo add knockback and dont damage the same enemy twice like from throwweapon

        }

        void Start()
        {
            Destroy(gameObject, destroyInNSeconds);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Player") { return; } 

            var healthSystem = col.gameObject.GetComponent<HealthSystem>();
            if (healthSystem) // hit an enemy
            {
                healthSystem.DamageHealth(damage);
            }
        }

    }
}