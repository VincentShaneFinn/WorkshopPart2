using System.Collections;
using UnityEngine;
using System.Collections.Generic;

using Finisher.Characters.Systems;
using System;
using Finisher.Characters.Systems.Strategies;

namespace Finisher.Characters.Enemies
{
    public class KnightLeaderAI : EnemyAI
    {

        [SerializeField] AnimationClip pointClip;
        [SerializeField] ParticleEvent feingOrder;
        [SerializeField] Transform orderHand;
        bool startTeamRushThinking = false;
        enum SpecialMoveState { Null, RushAttack, Retaliation }
        SpecialMoveState currentSpecialMoveState;
        IEnumerator teamRushCoroutine;

        protected override void Start()
        {
            base.Start();

            StartCoroutine(awakeFinalStand());
        }

        IEnumerator awakeFinalStand()
        {
            var orignalEnemies = squadManager.GetEnemies();
            yield return new WaitUntil(() => squadManager.GetEnemies().Count <= 1);

            yield return new WaitForSeconds(5f);

            int enemiesToRevive = 3;
            foreach (var enemy in orignalEnemies)
            {
                if (enemiesToRevive <= 0) { break; }
                if(enemy && !(enemy.GetComponent<EnemyAI>() is KnightLeaderAI))
                {
                    var combatSystem = enemy.GetComponent<KnightCombatSystem>();
                    if (combatSystem)
                    {
                        combatSystem.Revive();
                        enemiesToRevive--;
                    }
                }
            }

            StartCoroutine(awakeFinalStand());
        }
        protected override void pursuePlayer()
        {
            base.pursuePlayer();
            if (!startTeamRushThinking)
            {
                startTeamRushThinking = true;
                teamRushCoroutine = issueTeamRushAttack();
                StartCoroutine(teamRushCoroutine);
            }
        }

        protected override void endEngagePlayerSequence()
        {
            base.endEngagePlayerSequence();
            StopCoroutine(teamRushCoroutine);
            startTeamRushThinking = false;
        }

        private List<GameObject> enemies
        {
            get
            {
                return squadManager.GetEnemies();
            }
        }
        private List<KnightAI> enemiesThatRushed = new List<KnightAI>();

        IEnumerator issueTeamRushAttack()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(5,10));
            yield return StartCoroutine(teamRushAttackSequence());
            StartCoroutine(issueTeamRushAttack());
        }

        IEnumerator teamRushAttackSequence()
        {
            if(currentSpecialMoveState != SpecialMoveState.Null) { yield break; }
            else { currentSpecialMoveState = SpecialMoveState.RushAttack; }

            enemiesThatRushed = new List<KnightAI>();

            int numberOfRushers = 3;

            while (numberOfRushers > 0)
            {
                KnightAI knight = getAIForRushAttack(enemiesThatRushed);
                if (!knight) { yield break; }
                pointToKnight(knight.transform);

                float fientChance = .4f;
                bool useFeint = UnityEngine.Random.Range(0, 1f) <= fientChance;
                if (useFeint)
                {
                    feingOrder.Play(orderHand.position, orderHand.rotation);
                }
                knight.PerformRushAttack(useFeint);

                enemiesThatRushed.Add(knight);
                numberOfRushers--;
                yield return new WaitForSeconds(1.5f);
            }

            //Finally
            currentSpecialMoveState = SpecialMoveState.Null;
        }

        private void pointToKnight(Transform knight)
        {
            animOverrideSetter.SetTriggerOverride(AnimConstants.Parameters.BASIC_ACTION_TRIGGER, AnimConstants.OverrideIndexes.DEFAULT_BASIC_ACTION_INDEX, pointClip);
            if (knight)
            {
                character.LookAtTarget(knight, pointClip.length);
            }
        }

        private KnightAI getAIForRushAttack(List<KnightAI> alreadyRushedList)
        {
            squadManager.SortEnemiesByDistance();

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var knight = enemies[i].GetComponent<KnightAI>();
                if (knight)
                {
                    if (alreadyRushedList.Contains(knight) || noClearPathToTarget(knight.gameObject,combatTarget))
                    {
                        continue;
                    }
                    return knight;
                }
            }
            return null;
        }

        public void RetaliationRushAttackOrder()
        {
            if(currentSpecialMoveState != SpecialMoveState.Null) { return; }

            var tempEnemyList = new List<KnightAI>();

            int numberOfRushers = 2;
            while (numberOfRushers > 0)
            {
                KnightAI knight = getAIForRushAttack(tempEnemyList);
                if (!knight) { break; }

                float fientChance = .4f;
                bool useFeint = UnityEngine.Random.Range(0, 1f) <= fientChance;
                if (useFeint)
                {
                    feingOrder.Play(orderHand.position, orderHand.rotation);
                }
                knight.PerformRushAttack(useFeint);

                tempEnemyList.Add(knight);
                numberOfRushers--;
            }

            pointToKnight(null);
        }

        private bool noClearPathToTarget(GameObject knight, GameObject combatTarget)
        {

            int enemyLayerMask = 1 << LayerNames.EnemyLayer;

            enemyLayerMask = ~enemyLayerMask;

            RaycastHit hit;
            var heading = combatTarget.transform.position - knight.transform.position;

            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(knight.transform.position + knight.transform.up, heading, out hit, Mathf.Infinity, enemyLayerMask))
            {
                if (hit.transform.tag != TagNames.PlayerTag)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
