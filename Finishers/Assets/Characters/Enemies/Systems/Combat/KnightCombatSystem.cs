using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Enemies;
using System;

namespace Finisher.Characters.Systems
{
    public class KnightCombatSystem : CombatSystem
    {

        public bool IsPerformingSpecialAttack;
        [SerializeField] private float feintTime = .25f;
        private bool useFeint = false;

        AICharacterController character;
        EnemyAI enemyAI;
        Transform target;

        protected override void Start()
        {
            base.Start();

            character = GetComponent<AICharacterController>();
            enemyAI = GetComponent<EnemyAI>();
            var behaviors = animator.GetBehaviours<RushSequenceSMB>();
            foreach (var behavior in behaviors)
            {
                behavior.KnightCombatSystem = this;
            }
        }

        public void RushAttack(Transform target, bool feint)
        {
            this.target = target;
            useFeint = feint;
            animator.SetTrigger("SpecialAttack");
        }

        public IEnumerator RushingCoroutine()
        {
            yield return null; // need to wait a frame for EnemyAI to shut itself down

            GetComponent<AICharacterController>().toggleAgent(false);
            character.LookAtTarget(target);

            while (!enemyAI.isPlayerInAttackRange() && !obstacleAhead())
            {
                character.LookAtTarget(target);
                yield return null;
            }

            animator.SetTrigger(AnimConstants.Parameters.ATTACK_TRIGGER);
        }

        private bool obstacleAhead()
        {
            var distance = 1;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            int walkableLayerMask = 1 << LayerNames.WalkableLayer;
            int obstacleLayerMask = 1 << LayerNames.ObstacleLayer;


            if (Physics.Raycast(transform.position + Vector3.up, fwd, distance, walkableLayerMask) ||
                Physics.Raycast(transform.position + Vector3.up, fwd, distance, obstacleLayerMask) )
            {
                return true;
            }

            return false;
        }

        public void ResetRushing()
        {
            //do something when you have left the rushing state
            GetComponent<AICharacterController>().toggleAgent(true);
        }

        public IEnumerator MonitorSpecialAttackStatus()
        {
            IsPerformingSpecialAttack = true;

            yield return new WaitUntil(() => !animator.IsInTransition(0));

            yield return null;

            yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimConstants.Tags.SPECIAL_ATTACK_SEQUENCE_TAG));

            ResetRushing();
            IsPerformingSpecialAttack = false;
            StopAllCoroutines();
        }

        void FientPeriod()
        {
            if(useFeint) { 
                StartCoroutine(fient());
            }
        }

        IEnumerator fient()
        {
            var count = feintTime;

            while (count > 0)
            {
                animator.speed = (float)(count / feintTime) / 4;
                count -= Time.deltaTime;
                yield return null;
            }

            count = feintTime;

            while (count > 0)
            {
                animator.speed = (1 - ((float)(count / feintTime) / 4));
                count -= Time.deltaTime;
                yield return null;
            }

            animator.speed = 1f;
        }

    }
}