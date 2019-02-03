using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Finisher/Sound/CharacterSoundConfig")]
public class CharacterSoundConfig : ScriptableObject
{
    [SerializeField] private SimpleAudioEvent footStep;
    [SerializeField] private SimpleAudioEvent getHit;
    [SerializeField] private SimpleAudioEvent swordSwing_1;
    [SerializeField] private SimpleAudioEvent swordSwing_2;
    [SerializeField] private SimpleAudioEvent swordSwing_3;
    [SerializeField] private SimpleAudioEvent swordSwing_4;
    public SimpleAudioEvent FootStep { get { return footStep; } }
    public SimpleAudioEvent GetHit { get { return getHit; } }
    public SimpleAudioEvent SwordSwing_First { get { return swordSwing_1; } }
    public SimpleAudioEvent SwordSwing_Second { get { return swordSwing_2; } }
    public SimpleAudioEvent SwordSwing_Third { get { return swordSwing_3; } }
    public SimpleAudioEvent SwordSwing_Fourth { get { return swordSwing_4; } }
}
