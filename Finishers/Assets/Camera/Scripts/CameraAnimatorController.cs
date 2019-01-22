using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckForInputs();
    }

    private void CheckForInputs()
    {
        bool _isGrabbing = animator.GetBool("isGrabbing");

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("Is Grabbing: " + !_isGrabbing);
            animator.SetBool("isGrabbing", !_isGrabbing);
        }
    }
}
