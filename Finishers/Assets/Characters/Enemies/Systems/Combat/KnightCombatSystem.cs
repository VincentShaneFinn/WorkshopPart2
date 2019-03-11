using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Enemies;
using System;
using Finisher.Characters.Player.Finishers;

namespace Finisher.Characters.Systems
{
    public class KnightCombatSystem : CombatSystem
    {

        public bool IsPerformingSpecialAttack = false;
        public bool IsPerformingRangedAttack = false;
        [SerializeField] private float feintTime = .25f;
        private bool useFeint = false;

        [SerializeField] ThrowingOrb orb;
        RangedAttackSMB rangedSMB;

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
            rangedSMB = animator.GetBehaviour<RangedAttackSMB>();
            rangedSMB.RangeExitListeners += StopRangedAttack;
        }

        //void OnDestroy()
        //{
        //    rangedSMB.RangeExitListeners -= StopRangedAttack;
        //}

        #region RushAttack

        public void RushAttack(Transform target, bool feint)
        {
            this.target = target;
            useFeint = feint;
            animator.SetTrigger("SpecialAttack");
            animator.SetInteger("SpecialAttackIndex", 0);
        }

        private IEnumerator resetSpecialAttackTrigger()
        {
            yield return new WaitForSeconds(.5f);
            animator.ResetTrigger("SpecialAttack");
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

        #endregion

        #region RangedAttack

        public void RangedAttack()
        {
            animator.SetTrigger("SpecialAttack");
            animator.SetInteger("SpecialAttackIndex", 1);
        }

        IEnumerator facePlayerCoroutine;

        void StartRangedAttack()
        {
            //Not needed to do anything yet
            facePlayerCoroutine = facePlayer();
            StartCoroutine(facePlayerCoroutine);
        }

        IEnumerator facePlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            while (true)
            {
                transform.LookAt(player.transform);
                yield return null;
            }
        }

        void LaunchRangedAttack()
        {
            Instantiate(orb, transform.position + transform.forward + transform.up, transform.rotation);
            if (facePlayerCoroutine != null)
            {
                StopCoroutine(facePlayerCoroutine);
            }
        }

        void StopRangedAttack()
        {
            if (facePlayerCoroutine != null)
            {
                StopCoroutine(facePlayerCoroutine);
            }
        }

        #endregion

    }
}