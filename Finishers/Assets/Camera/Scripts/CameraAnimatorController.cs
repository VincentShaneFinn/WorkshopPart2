using System;
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
            playerFinisherSystem.OnFinisherExecutionSlice += quickZoomOutTrigger;
        }
         
        animator = GetComponent<Animator>();
    }

    void OnDestroy()
    {
        if (playerFinisherSystem)
        {
            playerFinisherSystem.OnGrabbingTargetToggled -= cameraZoomOnGrab;
            playerFinisherSystem.OnFinisherExecutionSlice -= quickZoomOutTrigger;
        }
    }

    private void cameraZoomOnGrab(bool isGrabbing)
    {
        if (isGrabbing)
        {
            animator.SetBool("isGrabbing", true);
        }
        else
        {
            animator.SetBool("isGrabbing", false);
        }
    }

    private void quickZoomOutTrigger()
    {
        animator.SetTrigger("ExecutionSlice");
    }
}
