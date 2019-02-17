using System.Collections;
using UnityEngine;
using System.Collections.Generic;

using Finisher.Characters.Systems;
using System;

namespace Finisher.Characters.Enemies
{
    public class KnightLeaderAI : EnemyAI
    {

        [SerializeField] AnimationClip pointClip;
        bool startTeamRushThinking = false;
        IEnumerator teamRushCoroutine;

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
            enemiesThatRushed = new List<KnightAI>();

            int numberOfRushers = 3;

            while (numberOfRushers > 0)
            {
                KnightAI knight = getAIForRushAttack();
                if (!knight) { yield break; }
                pointToKnight(knight.transform);
                knight.PerformRushAttack();
                enemiesThatRushed.Add(knight);
                numberOfRushers--;
                yield return new WaitForSeconds(1.5f);
            }
        }

        private void pointToKnight(Transform knight)
        {
            animOverrideSetter.SetTriggerOverride(AnimConstants.Parameters.BASIC_ACTION_TRIGGER, AnimConstants.OverrideIndexes.DEFAULT_BASIC_ACTION_INDEX, pointClip);
            character.LookAtTarget(knight, pointClip.length);
        }

        private KnightAI getAIForRushAttack()
        {
            if (enemies.Count <= squadManager.DirectAttackers + squadManager.IndirectAttackers) { return null; }

            squadManager.SortEnemiesByDistance();

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var knight = enemies[i].GetComponent<KnightAI>();
                if (knight)
                {
                    if (enemiesThatRushed.Contains(knight) || noClearPathToTarget(knight.gameObject,combatTarget))
                    {
                        continue;
                    }
                    return knight;
                }
            }
            return null;
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
