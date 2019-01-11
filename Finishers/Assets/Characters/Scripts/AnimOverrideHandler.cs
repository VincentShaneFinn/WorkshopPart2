using System.Collections.Generic;
using UnityEngine;

public class AnimOverrideHandler : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController animOverrideControllerConfig;
    private AnimatorOverrideController animOverrideController;
    private Animator animator;

    void Start() {

        animator = GetComponent<Animator>();

        animOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animOverrideController;

        //fill new override with data
        var dataOverride = animOverrideControllerConfig;

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(dataOverride.overridesCount);
        dataOverride.GetOverrides(overrides);

        //Apply it
        animOverrideController.ApplyOverrides(overrides);
    }

    public void SetAnimOverrideControllerConfig(AnimatorOverrideController aoc)
    {
        animOverrideController = aoc;
    }

    #region Public Interface for Overriding Animation Clips

    public void SetOverride(string overrideIndex, AnimationClip animClip)
    {
        animOverrideController[overrideIndex] = animClip;
    }

    public void SetFloatOverride(string floatName, float floatValue, string overrideIndex, AnimationClip animClip)
    {
        animOverrideController[overrideIndex] = animClip;
        animator.SetFloat(floatName, floatValue);
    }

    public void SetIntegerOverride(string intName, int intValue, string overrideIndex, AnimationClip animClip)
    {
        animOverrideController[overrideIndex] = animClip;
        animator.SetInteger(intName, intValue);
    }

    public void SetBoolOverride(string boolName, bool boolValue, string overrideIndex, AnimationClip animClip)
    {
        animOverrideController[overrideIndex] = animClip;
        animator.SetBool(boolName, boolValue);
    }

    public void SetTriggerOverride(string TriggerName, string OverrideIndex, AnimationClip AnimClip)
    {
        animOverrideController[OverrideIndex] = AnimClip;
        animator.SetTrigger(TriggerName);
    }

    #endregion
}
