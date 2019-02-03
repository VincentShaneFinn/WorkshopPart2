using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Systems;

public class CharacterSoundHandler : MonoBehaviour
{

    [SerializeField] CharacterSoundConfig config;

    protected HealthSystem healthSystem;

    private AudioSource baseAudioSource;
    private Rigidbody rigidBody;
    private const float CHECK_IF_MOVING = .3f;

    void Awake()
    {
        baseAudioSource = gameObject.AddComponent<AudioSource>();
        healthSystem = GetComponent<HealthSystem>();

        if (healthSystem)
        {
            healthSystem.OnDamageTaken += GetHit;
        }
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void OnDestroy()
    {
        if (healthSystem)
        {
            healthSystem.OnDamageTaken -= GetHit;
        }
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
    #endregion

    #region Combat Animation Events

    void GetHit()
    {
        config.GetHit.Play(baseAudioSource);
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
