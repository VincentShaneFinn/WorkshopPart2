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

    public AK.Wwise.Event swordSwinging;
    public AK.Wwise.Event grabThing;
    public AK.Wwise.Event grabContact;
    public AK.Wwise.Event finisherSlice;
    public AK.Wwise.Event aoeBlast;





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
            //else if (terrainType == "Floor")
            else
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
        swordSwinging.Post(gameObject);

    }

    void SwordSwing_2()
    {
        config.SwordSwing_Second.Play(baseAudioSource);
        swordSwinging.Post(gameObject);

    }

    void SwordSwing_3()
    {
        config.SwordSwing_Third.Play(baseAudioSource);
        swordSwinging.Post(gameObject);

    }

    void SwordSwing_4()
    {
        config.SwordSwing_Fourth.Play(baseAudioSource);
        swordSwinging.Post(gameObject);

    }

    void HeavySwing_1()
    {
        config.HeavySwordSwing_First.Play(baseAudioSource);
        swordSwinging.Post(gameObject);

    }

    void HeavySwing_2()
    {
        config.HeavySwordSwing_Second.Play(baseAudioSource);
        swordSwinging.Post(gameObject);

    }

    void Dagger_Light()
    {
        config.DaggerLight.Play(baseAudioSource);
        swordSwinging.Post(gameObject);

    }

    void Dagger_Heavy()
    {
        config.DaggerHeavy.Play(baseAudioSource);
        swordSwinging.Post(gameObject);

    }

    void Finisher_Grab()
    {
        config.FinisherGrab.Play(baseAudioSource);
        grabThing.Post(gameObject);

    }

    void Finisher_Grab_Contact()
    {
        config.FinisherGrabContact.Play(baseAudioSource);
        grabContact.Post(gameObject);

    }

    #endregion

    void FinisherSlice()
    {
        config.FinisherSlice.Play(baseAudioSource);
        finisherSlice.Post(gameObject);
    }
    //TODO move this to the prefab
    void Finisher_AOE_Blast()
    {
        AudioSource audioSourceToKill = gameObject.AddComponent<AudioSource>();
        config.FinisherAOEBlast.Play(audioSourceToKill);
        aoeBlast.Post(gameObject);
        Destroy(audioSourceToKill, audioSourceToKill.clip.length);
    }

    void OnCollisionStay(Collision collision)
    {
        terrainType = collision.gameObject.tag;
    }
}
