using UnityEngine;

namespace Finisher.Characters.Systems
{
    public class PlayerHealthSystem : HealthSystem
    {
        public delegate void KnockedBack();
        public event KnockedBack OnKnockBack;
        private void CallKnockbackEvent()
        {
            if (OnKnockBack != null)
            {
                OnKnockBack();
            }
        }

        protected override void Start()
        {

            setPlayerHealthSlider();

            base.Start();
        }

        private void setPlayerHealthSlider()
        {
            if (gameObject.tag == "Player")
            {
                healthSlider = FindObjectOfType<UI.PlayerUIObjects>().HealthSlider;
            }
        }

        #region override Knockback

        public override void Knockback(AnimationClip animClip)
        {
            base.Knockback(animClip);
            CallKnockbackEvent();
        }

        #endregion

    }
}
