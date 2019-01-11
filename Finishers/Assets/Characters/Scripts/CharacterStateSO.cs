using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = "Finisher/States/CharacterState")]
    public class CharacterStateSO : ScriptableObject
    {

        //public void Initialize(Animator anim)
        //{
        //    DyingBool = new DyingBool(anim);
        //}

        // todo create property drawer to allow preview of setting dying, so that the delegate event gets called
        [SerializeField] public DyingBool DyingBool;
        [SerializeField] public bool Grabbing;

    }
}
