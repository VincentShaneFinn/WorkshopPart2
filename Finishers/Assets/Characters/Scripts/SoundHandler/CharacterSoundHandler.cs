using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Systems;
using System;

public class CharacterSoundHandler : MonoBehaviour
{

    [SerializeField] CharacterSoundConfig config;

    protected HealthSystem healthSystem;

    private AudioSource baseAudioSource;
    private Rigidbody rigidBody;
    private const float CHECK_IF_MOVING = .3f;

    private string terrainType;

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
        Footstep();
    }

    void FootR()
    {
        Footstep();
    }

    void Footstep()
    {
        RaycastHit hit;

        if (rigidBody.velocity.magnitude > CHECK_IF_MOVING)
        {
            if (terrainType == "SandFloor")
            {
                config.FootStepSand.Play(baseAudioSource);
            }
            else if (terrainType == "Floor")
            {
                config.FootStepDefault.Play(baseAudioSource);
            }
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

    void HeavySwing_1()
    {
        config.HeavySwordSwing_First.Play(baseAudioSource);
    }

    void HeavySwing_2()
    {
        config.HeavySwordSwing_Second.Play(baseAudioSource);
    }

    void Dagger_Light()
    {
        config.DaggerLight.Play(baseAudioSource);
    }

    void Dagger_Heavy()
    {
        config.DaggerHeavy.Play(baseAudioSource);
    }

    #endregion

    void FinisherSlice()
    {
        config.FinisherSlice.Play(baseAudioSource);
    }

    //TODO move this to the prefab
    void Finisher_AOE_Blast()
    {
        AudioSource audioSourceToKill = gameObject.AddComponent<AudioSource>();
        config.FinisherAOEBlast.Play(audioSourceToKill);
        Destroy(audioSourceToKill, audioSourceToKill.clip.length);
    }

    void OnCollisionStay(Collision collision)
    {
        terrainType = collision.gameObject.tag;
    }
}
