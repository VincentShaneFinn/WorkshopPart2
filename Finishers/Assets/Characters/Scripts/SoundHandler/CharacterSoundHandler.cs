using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundHandler : MonoBehaviour
{

    [SerializeField] CharacterSoundConfig config;

    private AudioSource baseAudioSource;
    private Rigidbody rigidBody;
    private const float CHECK_IF_MOVING = .3f;

    void Awake()
    {
        baseAudioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    #region Movement Animation Events

    void FootL()
    {
        if (rigidBody.velocity.magnitude > CHECK_IF_MOVING)
        {
            config.FootStep.Play(baseAudioSource);
        }
    }

    void FootR()
    {
        if (rigidBody.velocity.magnitude > CHECK_IF_MOVING)
        {
            config.FootStep.Play(baseAudioSource);
        }
    }

    void SwordSwing_1()
    {
        config.SwordSwing_First.Play(baseAudioSource);
    }

    void SwordSwing_2()
    {
        config.SwordSwing_Second.Play(baseAudioSource);
    }

    void SwordSwing_3()
    {
        config.SwordSwing_Third.Play(baseAudioSource);
    }

    void SwordSwing_4()
    {
        config.SwordSwing_Fourth.Play(baseAudioSource);
    }

    #endregion
}
