using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Finisher/Sound/CharacterSoundConfig")]
public class CharacterSoundConfig : ScriptableObject
{
    [SerializeField] private SimpleAudioEvent footStep; 
    public SimpleAudioEvent FootStep { get { return footStep; } }
}
