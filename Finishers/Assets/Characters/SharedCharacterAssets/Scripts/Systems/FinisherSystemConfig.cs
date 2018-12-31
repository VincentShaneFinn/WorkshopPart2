using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = ("Finisher/FinisherSystem"))]
    public class FinisherSystemConfig : ScriptableObject
    {
        [SerializeField] AnimationClip light1Animation; public AnimationClip Light1Animation { get { return light1Animation; } private set { } }
        [SerializeField] AnimationClip light2Animation; public AnimationClip Light2Animation { get { return light2Animation; } private set { } }
        [SerializeField] AnimationClip light3Animation; public AnimationClip Light3Animation { get { return light3Animation; } private set { } }
        [SerializeField] AnimationClip light4Animation; public AnimationClip Light4Animation { get { return light4Animation; } private set { } }
        [SerializeField] float lightAttackDamage = 10f; public float LightAttackDamage { get { return lightAttackDamage; } private set { } }
        [SerializeField] float heavyAttackDamage = 20f; public float HeavyAttackDamage { get { return heavyAttackDamage; } private set { } }
    }
}