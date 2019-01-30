using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Finisher/Sound/CharacterSoundConfig")]
public class CharacterSoundConfig : ScriptableObject
{
    [SerializeField] private SimpleAudioEvent footStep;
    [SerializeField] private SimpleAudioEvent swordSwing;
    public SimpleAudioEvent FootStep { get { return footStep; } }
    public SimpleAudioEvent SwordSwing {get { return swordSwing; } }
}
