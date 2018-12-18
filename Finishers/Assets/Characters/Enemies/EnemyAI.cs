using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    [RequireComponent(typeof(AICharacterController))]
    public class EnemyAI : MonoBehaviour
    {

        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float attackRadius = 1.5f;

        AICharacterController aiCharacter;
        GameObject player = null;

        // Use this for initialization
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            aiCharacter = GetComponent<AICharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (player == null){ return; }

            // todo make a state machine
            pursueNearbyPlayer();
            attackPlayerIfNear();
        }

        private void pursueNearbyPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= chaseRadius)
            {
                aiCharacter.SetTarget(player.transform);
            }
            else
            {
                aiCharacter.SetTarget(transform);
            }
        }

        private void attackPlayerIfNear()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= attackRadius)
            {
                print("attack player");
            }
        }
    }
}
