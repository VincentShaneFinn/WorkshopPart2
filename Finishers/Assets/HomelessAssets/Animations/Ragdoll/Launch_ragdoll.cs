using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch_ragdoll : MonoBehaviour {

    public float force;
    public GameObject hips;

	// Use this for initialization
	void Start () {
        hips.GetComponent<Rigidbody>().AddForce(transform.up * force);
        Debug.Log(hips);
    }
	
	// Update is called once per frame
	void Update () {

    }
}
