namespace Finisher.Characters
{
    public static class AnimConstants
    {
        public class States
        {
            public const string STRAFING_LOCOMOTION_STATE = "Strafing Locomotion";
            public const string BASIC_LOCOMOTION_STATE = "Basic Locomotion";
            public const string RUNNING_STATE = "Running";
            public const string AIRBORNE_STATE = "Airborne";
            public const string STRAFING_STATE = "Strafing Locomotion";
            public const string KNOCKBACK_STATE = "Knockback";
            public const string STUNNED_STATE = "Stunned";
            public const string INVULNERABLEACTION_STATE = "Invulnerable Action";
            public const string LIGHT_ATTACK1_STATE = "Light1";
            public const string LIGHT_ATTACK2_STATE = "Light2";
            public const string LIGHT_ATTACK3_STATE = "Light3";
            public const string LIGHT_ATTACK4_STATE = "Light4";
            public const string HEAVY_ATTACK1_STATE = "Heavy1";
            public const string HEAVY_ATTACK2_STATE = "Heavy2";
            public const string FINISHER_EXECUTE_STATE = "FinisherExecute";
            public const string FINISHER_SELECTION_STATE = "FinisherSelection";
            public const string FINISHER_ACTIVATION_STATE = "FinisherActivation";
            public const string DODGE_STATE = "Dodge";
            public const string PARRY_STATE = "Parry";
            public const string DYING_STATE = "Dying";
        }

        public class Tags
        {
            public const string LOCOMOTION_TAG = "Locomotion";
            public const string LIGHTATTACK_TAG = "LightAttack";
            public const string HEAVYATTACK_TAG = "HeavyAttack";
            public const string UNINTERUPTABLE_TAG = "Uninteruptable";
            public const string INVULNERABLE_SEQUENCE_TAG = "InvulnerableSequence";
        }

        public class Parameters
        {
            public const string FORWARD_FLOAT = "Forward";
            public const string TURN_FLOAT = "Turn";
            public const string JUMP_FLOAT = "Jump";
            public const string FORWARDLEG_FLOAT = "ForwardLeg";

            public const string ONGROUND_BOOL = "OnGround";
            public const string STRAFING_BOOL = "Strafing";
            public const string ISHEAVY_BOOL = "IsHeavy";
            public const string FINISHERMODE_BOOL = "FinisherMode";
            public const string STUNNED_BOOL = "Stunned";
            public const string DYING_BOOL = "Dying";

            public const string ATTACK_TRIGGER = "Attack";
            public const string FINISHER_EXECUTION_TRIGGER = "FinisherExecution";
            public const string DODGE_TRIGGER = "Dodge";
            public const string PARRY_TRIGGER = "Parry";
            public const string INVULNERABLEACTION_TRIGGER = "InvulnerableAction";
            public const string KNOCKBACK_TRIGGER = "Knockback";
            public const string RESETFORCEFULLY_TRIGGER = "ResetForcefully";
            public const string RESETPEACEFULLY_TRIGGER = "ResetPeacefully";

            public const string ATTACK_SPEED_MULTIPLIER = "AttackSpeedMultiplier";
            public const string MOVEMENT_SPEED_MULTIPLIER = "MovementSpeedMultiplier";
        }

        public static class OverrideIndexes
        {
            public const string KNOCKBACK_INDEX = "DEFAULT_KNOCKBACK";
            public const string DODGE_INDEX = "DEFAULT_DODGE";
            public const string DEATH_INDEX = "DEFAULT_DEATH";
            public const string INVULNERABLEACTION_INDEX = "DEFAULT_INVULNERABLE_ACTION";
            public const string FINISHER_ACTIVATION_INDEX = "DEFAULT_FINISHER_ACTIVATION";
        }
    }
}
