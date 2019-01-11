using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Finisher.Characters
{
    public delegate void CharacterIsDying();

    public abstract class CharacterState : MonoBehaviour
    {
        [HideInInspector] public Animator Animator;

        void Start()
        {
            Animator = GetComponent<Animator>();
            Assert.IsNotNull(Animator);
        }

        // while Dying is an aniamtion tree parameter, it should only be set through here, not via the animator
        public abstract bool Dying { get; set; }
        public abstract void SubscribeToDeathEvent(CharacterIsDying method);
        public abstract void UnsubscribeToDeathEvent(CharacterIsDying method);
    }
}