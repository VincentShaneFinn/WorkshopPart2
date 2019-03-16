using UnityEngine;
using UnityEngine.UI;

using Finisher.UI;
using Finisher.UI.Meters;
using Finisher.Characters.Systems;
using System.Collections;
using Finisher.Cameras;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Enemies.Systems
{
    public class EnemyLeaderHealthSystem : EnemyHealthSystem
    {
        [SerializeField] private int hitsUntilRetaliation = 2;
        [SerializeField] private float timeToResetHitsTaken = 2f;
        private int hitsTaken = 0;
        private IEnumerator resetHitsTakenCoroutine;

        private Animator animator;
        private CameraAnimatorController cameraAnimatorController;
        private CameraLookController cameraLookController;

        //Override get hit to play parry animation and stun the player, then riposte after a fiew
        protected override void Start()
        {
            base.Start();
            resetHitsTakenCoroutine = resetHitsTaken();
        }

        public override void DamageHealth(float damage, DamageSystem damageSource)
        {
            base.DamageHealth(damage, damageSource);
            if (damageSource is CoreCombatDamageSystem && !characterState.Stunned )
            {
                hitsTaken++;
                StopCoroutine(resetHitsTakenCoroutine);
                if (hitsTaken >= hitsUntilRetaliation)
                {
                    hitsTaken = 0;
                    GetComponent<KnightLeaderAI>().RetaliationRushAttackOrder();
                    StopCoroutine(resetHitsTakenCoroutine);
                }
                StartCoroutine(resetHitsTakenCoroutine);
            }
        }

        public override void Kill(AnimationClip animClip, bool overrideKillAnim = false)
        {
            base.Kill(animClip, overrideKillAnim);
            toggleEnemyCanvas(false);
            transform.parent.gameObject.GetComponent<SquadManager>().killEnemies();
            
        }

        IEnumerator resetHitsTaken()
        {
            yield return new WaitForSeconds(timeToResetHitsTaken);
            hitsTaken = 0;
        }

    }
}