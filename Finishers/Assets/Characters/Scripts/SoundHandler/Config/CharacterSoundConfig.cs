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
    [SerializeField] private SimpleAudioEvent heavySwordSwing_1;
    [SerializeField] private SimpleAudioEvent heavySwordSwing_2;
    [SerializeField] private SimpleAudioEvent daggerLight;
    [SerializeField] private SimpleAudioEvent daggerHeavy;
    [SerializeField] private SimpleAudioEvent finisherSlice;
    [SerializeField] private SimpleAudioEvent finisherAOEBlast;
    public SimpleAudioEvent FootStep { get { return footStep; } }
    public SimpleAudioEvent GetHit { get { return getHit; } }
    public SimpleAudioEvent SwordSwing_First { get { return swordSwing_1; } }
    public SimpleAudioEvent SwordSwing_Second { get { return swordSwing_2; } }
    public SimpleAudioEvent SwordSwing_Third { get { return swordSwing_3; } }
    public SimpleAudioEvent SwordSwing_Fourth { get { return swordSwing_4; } }
    public SimpleAudioEvent HeavySwordSwing_First { get { return heavySwordSwing_1; } }
    public SimpleAudioEvent HeavySwordSwing_Second { get { return heavySwordSwing_2; } }
    public SimpleAudioEvent DaggerLight { get { return daggerLight; } }
    public SimpleAudioEvent DaggerHeavy { get { return daggerHeavy; } }
    public SimpleAudioEvent FinisherSlice { get { return finisherSlice; } }
    public SimpleAudioEvent FinisherAOEBlast { get { return finisherAOEBlast; } }
}
