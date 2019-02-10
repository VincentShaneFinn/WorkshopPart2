using UnityEngine;
using UnityEngine.UI;

using Finisher.UI;
using Finisher.UI.Meters;
using Finisher.Characters.Systems;
using System.Collections;
using Finisher.Cameras;

namespace Finisher.Characters.Enemies.Systems
{
    public class TestEnemyHealthSystem : EnemyHealthSystem
    {
        [SerializeField] private CombatConfig combatConfig;

        private Animator animator;
        private CameraAnimatorController cameraAnimatorController;
        private CameraLookController cameraLookController;
        //Override get hit to play parry animation and stun the player, then riposte after a fiew
        protected override void Start()
        {
            base.Start();
            OnDamageTaken += parry;
            animator = GetComponent<Animator>();
            cameraAnimatorController = GameObject.FindObjectOfType<CameraAnimatorController>();
            cameraLookController = GameObject.FindObjectOfType<CameraLookController>();
        }

        void OnDestroy()
        {
            OnDamageTaken -= parry;
        }

        public override void DamageHealth(float damage)
        {
            CallDamageTakenEvent();
            return;
        }

        private void parry()
        {
            //animator.SetTrigger(AnimConstants.Parameters.PARRY_TRIGGER);
            attack();
            GameObject.FindGameObjectWithTag(TagNames.PlayerTag).GetComponent<CharacterState>().Stun(30f);
            cameraLookController.SetFollowTarget(null);
        }

        private void attack()
        {
            Time.timeScale = .5f;
            animator.SetTrigger(AnimConstants.Parameters.RESETFORCEFULLY_TRIGGER);
            animator.SetTrigger(AnimConstants.Parameters.FINISHER_EXECUTION_TRIGGER);
            cameraAnimatorController.GetComponent<Animator>().SetTrigger("FinishingBlow");
            FindObjectOfType<CutscenePlayer>().PlayCutscene();
        }

        //animation event

        void FinisherExecutionSlice()
        {
            GameObject.FindGameObjectWithTag(TagNames.PlayerTag).GetComponent<HealthSystem>().CutInHalf();
        }

    }
}