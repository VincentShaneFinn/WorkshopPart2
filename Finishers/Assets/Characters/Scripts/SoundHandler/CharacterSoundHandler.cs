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

    #endregion
}
