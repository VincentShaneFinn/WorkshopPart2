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
        var _isGrabbing = animator.GetBool("isGrabbing");
        var _transition_ID = animator.GetInteger("transition_ID");

        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            Debug.Log("Is Grabbing: " + !_isGrabbing);
            animator.SetBool("isGrabbing", !_isGrabbing);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            Debug.Log("Current Transition: " + _transition_ID % 2);

            if (_transition_ID != 1)
            {
                animator.SetInteger("transition_ID", 1);
            }
            else
            {
                animator.SetInteger("transition_ID", 2);
            }
        }
    }
}
