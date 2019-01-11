using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Finisher.Characters
{
    public delegate void CharacterIsDying();

    public class CharacterState : MonoBehaviour
    {
        [HideInInspector] public Animator Animator;

        protected void Awake()
        {
            Animator = GetComponent<Animator>();
            Assert.IsNotNull(Animator);

            initialize();
        }

        protected virtual void initialize()
        {
            DyingBool = new DyingBool(Animator);
        }

        // while Dying is an aniamtion tree parameter, it should only be set through here, not via the animator
        public virtual DyingBool DyingBool { get; set; }
        public bool Dying { get { return DyingBool.Dying; } set { DyingBool.Dying = value; } } 


    }
}