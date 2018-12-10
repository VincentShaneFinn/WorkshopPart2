using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finisher.Characters;

public class Enemy : MonoBehaviour {

    [SerializeField] float attackRadius = 5f;

    AIMovementController aiCharacterController;
    GameObject player = null;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        aiCharacterController = GetComponent<AIMovementController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (player)
        {
            pursueNearbyPlayer();
        }
    }

    private void pursueNearbyPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer <= attackRadius)
        {
            aiCharacterController.SetTarget(player.transform);
        }
        else
        {
            aiCharacterController.SetTarget(transform);
        }
    }
}
