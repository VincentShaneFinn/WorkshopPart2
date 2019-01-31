using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundHandler : MonoBehaviour
{

    [SerializeField] CharacterSoundConfig config;

    private AudioSource baseAudioSource;

    void Awake()
    {
        baseAudioSource = gameObject.AddComponent<AudioSource>();
    }

    #region Movement Animation Events

    void FootL()
    {
        config.FootStep.Play(baseAudioSource);
    }

    void FootR()
    {
        config.FootStep.Play(baseAudioSource);
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
