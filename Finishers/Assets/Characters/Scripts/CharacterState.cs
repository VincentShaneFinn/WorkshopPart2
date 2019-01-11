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

        protected void initialize()
        {
            DyingBool = new DyingBool(Animator);
            Grabbing = false;
        }

        // The core idea is that something that may want to be visable from an external class should be added to the Character State From SO class overrides

        #region States Stored in Scriptable Object

        // while Dying is an aniamtion tree parameter, it should only be set through here, not via the animator
        public virtual DyingBool DyingBool { get; set; }

        public virtual bool Grabbing { get; set; }

        #endregion

        #region Public Interface

        public bool Dying { get { return DyingBool.Dying; } set { DyingBool.Dying = value; } }

        #endregion


    }
}