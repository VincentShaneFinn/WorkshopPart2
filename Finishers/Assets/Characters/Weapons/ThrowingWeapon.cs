﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeapon : MonoBehaviour
{
    [SerializeField] float moveSpeed = 20f;

    private BoxCollider boxCollider;
    private Rigidbody rigidBody;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = true; // todo what to do with the collider

        rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        
    }

    void Update()
    {
        rigidBody.velocity = transform.forward * moveSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Enemy")
        {
            moveSpeed = 0;
            transform.position = transform.position;
        }
        //else
        //{
        //    Physics.IgnoreLayerCollision()
        //}
    }
}
