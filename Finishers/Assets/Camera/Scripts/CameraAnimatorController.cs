﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Systems;

public class CameraAnimatorController : MonoBehaviour
{
    private Animator animator;
    private FinisherSystem playerFinisherSystem;

    void Start()
    {
        playerFinisherSystem = GameObject.FindGameObjectWithTag(TagNames.PlayerTag).GetComponent<FinisherSystem>();

        if (playerFinisherSystem)
        {
            playerFinisherSystem.OnGrabbingTargetToggled += cameraZoomOnGrab;
        }
         
        animator = GetComponent<Animator>();
    }

    void OnDestroy()
    {
        if (playerFinisherSystem)
        {
            playerFinisherSystem.OnGrabbingTargetToggled -= cameraZoomOnGrab;
        }
    }

    private void cameraZoomOnGrab(bool isGrabbing)
    {
        var _isGrabbing = animator.GetBool("isGrabbing");
        var _transition_ID = animator.GetInteger("transition_ID");

        if (isGrabbing)
        {
            Debug.Log("Is Grabbing: " + !_isGrabbing);
            animator.SetBool("isGrabbing", true);
        }
        else
        {
            Debug.Log("Is Grabbing: " + !_isGrabbing);
            animator.SetBool("isGrabbing", false);
        }
    }
}
