using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObjectRandomly : MonoBehaviour
{
    Rigidbody rigidBody;
    [SerializeField] float speed = 200f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 360), transform.eulerAngles.z);
        rigidBody.isKinematic = false;
        Vector3 force = transform.forward;
        force = new Vector3(force.x, 1, force.z);
        rigidBody.AddForce(force * speed);
        rigidBody.AddRelativeTorque(force * speed);

    }

}
