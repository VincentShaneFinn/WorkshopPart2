using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Characters
{
    [CreateAssetMenu(menuName = "Finisher/States/CharacterState")]
    public class CharacterStateSO : ScriptableObject
    {

        //The State if you really need it, or want the player gameobject through the so

        private CharacterState state;
        private HealthSystem healthSystem;
        private FinisherSystem finisherSystem;
        public HealthSystem CombatTarget = null;

        public CharacterState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                healthSystem = state.GetComponent<HealthSystem>();
                finisherSystem = state.GetComponent<FinisherSystem>();
            }
        }

        #region IsDying

        [Header("IsDying")]
        [SerializeField] bool isDyingInspectable = false;
        [SerializeField] bool useDyingInspectable = false;
        public bool IsDying {
            get {
                if (state && !useDyingInspectable)
                {
                    return state.DyingState.Dying;
                }
                else
                {
                    return isDyingInspectable;
                }
            }
        }

        #endregion

        #region IsGrabbing

        [Header("IsGrabbing")]
        [SerializeField] bool isGrabbingInspectable = false;
        [SerializeField] bool useGrabbingInspectable = false;
        public bool IsGrabbing
        {
            get
            {
                if (state && !useGrabbingInspectable)
                {
                    return state.Grabbing;
                } else
                {
                    return isGrabbingInspectable;
                }
            }
        }

        #endregion

        #region IsFinishing

        [Header("IsFinishing")]
        //NOTE: currently the exact same as IsInvulnerableSequence
        [SerializeField] bool isFinishingInspectable = false;
        [SerializeField] bool useFinishingInspectable = false;
        public bool IsFinishing
        {
            get
            {
                if (state && !useFinishingInspectable)
                {
                    return state.IsInvulnerableSequence;
                }
                else
                {
                    return isFinishingInspectable;
                }
            }
        }

        #endregion

        #region IsInvulnerable

        [Header("IsInvulnerable")]
        [SerializeField] bool isInvulnerableInspectable = false;
        [SerializeField] bool useInvulnerableInspectable = false;
        public bool IsInvulnerable
        {
            get
            {
                if (state && !useInvulnerableInspectable)
                {
                    return state.Invulnerable;
                }
                else
                {
                    return isInvulnerableInspectable;
                }
            }
        }

        #endregion

        #region IsInvulnerableSequence

        [Header("IsInvulnerableSequence")]
        [SerializeField] bool isInvulnerableSequenceInspectable = false;
        [SerializeField] bool useInvulnerableSequenceInspectable = false;
        public bool IsInvulnerableSequence
        {
            get
            {
                if (state && !useInvulnerableSequenceInspectable)
                {
                    return state.IsInvulnerableSequence;
                }
                else
                {
                    return isInvulnerableSequenceInspectable;
                }
            }
        }

        #endregion

        #region IsAttacking

        [Header("IsAttacking")]
        [SerializeField] bool isAttackingInspectable = false;
        [SerializeField] bool useAttackingInspectable = false;
        public bool IsAttacking
        {
            get
            {
                if (state && !useAttackingInspectable)
                {
                    return state.Attacking;
                }
                else
                {
                    return isAttackingInspectable;
                }
            }
        }

        #endregion

        #region IsParrying

        [Header("IsParrying")]
        [SerializeField] bool isParryingInspectable = false;
        [SerializeField] bool useParryingInspectable = false;
        public bool IsParrying
        {
            get
            {
                if (state && !useParryingInspectable)
                {
                    return state.Parrying;
                }
                else
                {
                    return isParryingInspectable;
                }
            }
        }

        #endregion

        #region IsDodging

        [Header("IsDodging")]
        [SerializeField] bool isDodgingInspectable = false;
        [SerializeField] bool useDodgingInspectable = false;
        public bool IsDodging
        {
            get
            {
                if (state && !useDodgingInspectable)
                {
                    return state.Dodging;
                }
                else
                {
                    return isDodgingInspectable;
                }
            }
        }

        #endregion

        #region IsFinisherModeActive

        [Header("IsFinisherModeActive")]
        [SerializeField] bool isFinisherModeActiveInspectable = false;
        [SerializeField] bool useFinisherModeActiveInspectable = false;
        public bool IsFinisherModeActive
        {
            get
            {
                if (state && !useFinisherModeActiveInspectable)
                {
                    return state.FinisherModeActive;
                }
                else
                {
                    return isFinisherModeActiveInspectable;
                }
            }
        }

        #endregion

        #region GetCurrentHealthAsPercentage

        [Header("GetCurrentHealthAsPercentage")]
        [SerializeField] [Range(0, 1)] float currentHealthPercentInspectable = 0.5f;
        [SerializeField] bool useCurentHealthPercentInspectable = false;
        public float GetCurrentHealthAsPercentage()
        {
            if (healthSystem && !useCurentHealthPercentInspectable)
            {
                return healthSystem.GetHealthAsPercent();
            }
            else
            {
                return currentHealthPercentInspectable;
            }
        }

        #endregion

        #region GetFinisherMeterAsPercentage

        [Header("GetFinisherMeterAsPercentage")]
        [SerializeField] [Range(0, 1)] float finisherMeterPercentInspectable = 0.5f;
        [SerializeField] bool useFinisherMeterPercentInspectable = false;
        public float GetFinisherMeterAsPercentage()
        {
            if (finisherSystem && !useFinisherMeterPercentInspectable)
            {
                return finisherSystem.GetFinisherMeterAsPercent();
            }
            else
            {
                return finisherMeterPercentInspectable;
            }
        }

        #endregion

        #region GetCombatTargetVolatilityAsPercent

        [Header("GetCombatTargetVolatilityAsPercent")]
        [SerializeField] [Range(0, 1)] float combatTargetVolatilityPercent = 0.5f;
        [SerializeField] bool useCombatTargetVolatilityPercent = false;
        public float GetCombatTargetVolatilityAsPercent()
        {
            if (CombatTarget)
            {
                return CombatTarget.GetVolaitilityAsPercent();
            }
            else if (useCombatTargetVolatilityPercent)
            {
                return combatTargetVolatilityPercent;
            }
            else
            {
                return 0;
            }
        }

        #endregion

    }
}
