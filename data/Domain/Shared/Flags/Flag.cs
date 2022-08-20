using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared.Flags
{
    public class Flag : ValueObject<Flag>
    {
        public override string ToString()
        {
            return $"{nameof(Flag)}.{Value}";
        }

        public static class Condition
        {
            public static Flag BehaviourDoesNotExist => new Flag(Flags.ConditionBehaviourDoesNotExist);
            public static Flag BehaviourExists => new Flag(Flags.ConditionBehaviourExists);
            public static Flag TargetDoesNotHaveFullHealth => new Flag(Flags.ConditionTargetDoesNotHaveFullHealth);
            public static Flag NoActorsFoundFromEffect => new Flag(Flags.ConditionNoActorsFoundFromEffect);
            public static Flag TargetIsLowGround => new Flag(Flags.ConditionTargetIsLowGround);
            public static Flag TargetIsHighGround => new Flag(Flags.ConditionTargetIsHighGround);
            public static Flag TargetIsUnoccupied => new Flag(Flags.ConditionTargetIsUnoccupied);
            public static Flag TargetIsDifferentTypeThanOrigin => new Flag(Flags.ConditionTargetIsDifferentTypeThanOrigin);
        }

        public static class Amount
        {
            public static Flag FromMissingHealth => new Flag(Flags.AmountFromMissingHealth);
        }

        public static class Modification
        {
            public static Flag CannotBeHealed => new Flag(Flags.ModificationCannotBeHealed);
            public static Flag AbilitiesDisabled => new Flag(Flags.ModificationAbilitiesDisabled);
            public static Flag ClimbsDown => new Flag(Flags.ModificationClimbsDown);
            public static Flag MovesThroughEnemyUnits => new Flag(Flags.ModificationMovesThroughEnemyUnits);
            public static Flag MustExecuteAttack => new Flag(Flags.ModificationMustExecuteAttack);
            public static Flag CannotAttack => new Flag(Flags.ModificationCannotAttack);
            public static Flag MovementDisabled => new Flag(Flags.ModificationMovementDisabled);
            public static Flag CanAttackAnyTeam => new Flag(Flags.ModificationCanAttackAnyTeam);
            public static Flag IgnoreArmour => new Flag(Flags.ModificationIgnoreArmour);
            public static Flag ProvidesVision => new Flag(Flags.ModificationProvidesVision);
            public static Flag OnlyVisibleToAllies => new Flag(Flags.ModificationOnlyVisibleToAllies);
            public static Flag OnlyVisibleInFogOfWar => new Flag(Flags.ModificationOnlyVisibleInFogOfWar);
            
            /// <summary>
            /// Includes <see cref="MovementDisabled"/>, <see cref="CannotAttack"/>, <see cref="AbilitiesDisabled"/>.
            /// Action turn is always skipped.
            /// </summary>
            public static Flag FullyDisabled => new Flag(Flags.ModificationFullyDisabled);
        }

        public static class Effect
        {
            public static class ModifyPlayer
            {
                public static Flag GameLost => new Flag(Flags.EffectModifyPlayerGameLost);
            }

            public static class Search
            {
                public static Flag AppliedOnEnter => new Flag(Flags.EffectSearchAppliedOnEnter);
                /// <summary>
                /// Applies <see cref="Effects.Search"/> after <see cref="Durations.EndsAt"/>
                /// </summary>
                public static Flag AppliedOnAction => new Flag(Flags.EffectSearchAppliedOnAction);
                public static Flag RemovedOnExit => new Flag(Flags.EffectSearchRemovedOnExit);
            }
        }

        public static class Filter
        {
            public static Flag Origin => new Flag(Flags.FilterOrigin);
            public static Flag Source => new Flag(Flags.FilterSource);
            public static Flag Self => new Flag(Flags.FilterSelf);
            public static Flag Ally => new Flag(Flags.FilterAlly);
            public static Flag Enemy => new Flag(Flags.FilterEnemy);
            public static Flag Unit => new Flag(Flags.FilterUnit);
            public static Flag Structure => new Flag(Flags.FilterStructure);

            public static class Attribute
            {
                public static Flag Ranged => new Flag(Flags.FilterAttributeRanged);
            }
            
            public static class SpecificStructure
            {
                public static Flag Obelisk => new Flag(Flags.FilterSpecificStructureObelisk);
                public static Flag Hut => new Flag(Flags.FilterSpecificStructureHut);
            }

            public static class SpecificUnit
            {
                public static Flag Horrior => new Flag(Flags.FilterSpecificUnitHorrior);
                public static Flag Cannon => new Flag(Flags.FilterSpecificUnitCannon);
                public static Flag Ballista => new Flag(Flags.FilterSpecificUnitBallista);
                public static Flag Radar => new Flag(Flags.FilterSpecificUnitRadar);
                public static Flag Vessel => new Flag(Flags.FilterSpecificUnitVessel);
            }

            public static class SpecificFeature
            {
                public static Flag RadarRedDot => new Flag(Flags.FilterSpecificFeatureRadarRedDot);
            }
        }

        private Flag(Flags @enum)
        {
            Value = @enum;
        }

        private Flags Value { get; }

        private enum Flags
        {
            ConditionBehaviourDoesNotExist,
            ConditionBehaviourExists,
            ConditionTargetDoesNotHaveFullHealth,
            ConditionNoActorsFoundFromEffect,
            ConditionTargetIsLowGround,
            ConditionTargetIsHighGround,
            ConditionTargetIsUnoccupied,
            ConditionTargetIsDifferentTypeThanOrigin,
            AmountFromMissingHealth,
            ModificationCannotBeHealed,
            ModificationAbilitiesDisabled,
            ModificationClimbsDown,
            ModificationMovesThroughEnemyUnits,
            ModificationMustExecuteAttack,
            ModificationCannotAttack,
            ModificationMovementDisabled,
            ModificationCanAttackAnyTeam,
            ModificationIgnoreArmour,
            ModificationProvidesVision,
            ModificationFullyDisabled,
            ModificationOnlyVisibleToAllies,
            ModificationOnlyVisibleInFogOfWar,
            EffectModifyPlayerGameLost,
            EffectSearchAppliedOnEnter,
            EffectSearchAppliedOnAction,
            EffectSearchRemovedOnExit,
            FilterOrigin,
            FilterSource,
            FilterSelf,
            FilterAlly,
            FilterEnemy,
            FilterUnit,
            FilterStructure,
            FilterAttributeRanged,
            FilterSpecificStructureObelisk,
            FilterSpecificStructureHut,
            FilterSpecificUnitHorrior,
            FilterSpecificUnitCannon,
            FilterSpecificUnitBallista,
            FilterSpecificUnitRadar,
            FilterSpecificUnitVessel,
            FilterSpecificFeatureRadarRedDot
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
