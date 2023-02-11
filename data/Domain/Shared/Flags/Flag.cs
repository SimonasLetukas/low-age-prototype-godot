using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Entities.Features;
using low_age_data.Domain.Factions;

namespace low_age_data.Domain.Shared.Flags
{
    public class Flag : ValueObject<Flag>
    {
        public override string ToString()
        {
            return $"{nameof(Flag)}.{Value}";
        }

        public static class Effect
        {
            public static class ModifyPlayer
            {
                public static Flag GameLost => new(Flags.EffectModifyPlayerGameLost);
            }

            /// <summary>
            /// Used to control events when the effects should be applied or removed when <see cref="Search"/> is used
            /// as a <see cref="Passive.PeriodicEffect"/>. When counteracting the applied effects, any
            /// <see cref="Behaviours.Buff"/>s down the line are allowed to execute their
            /// <see cref="Behaviours.Buff.ConditionalEffects"/> before removal.
            /// </summary>
            public static class Search
            {
                /// <summary>
                /// Applies <see cref="Effects.Search"/> whenever a new actor enters the
                /// <see cref="Effects.Search.Shape"/>).
                /// </summary>
                public static Flag AppliedOnEnter => new(Flags.EffectSearchAppliedOnEnter);
                
                /// <summary>
                /// Applies <see cref="Effects.Search"/> after <see cref="Durations.EndsAt"/> (at the end of action
                /// for the <see cref="Flag.Filter.Source"/> <see cref="Entity"/> -- which issued this
                /// <see cref="Passive.PeriodicEffect"/>).
                /// </summary>
                public static Flag AppliedOnSourceAction => new(Flags.EffectSearchAppliedOnSourceAction);
                
                /// <summary>
                /// Applies <see cref="Effects.Search"/> after <see cref="Durations.EndsAt"/> for every action in
                /// action phase.
                /// </summary>
                public static Flag AppliedOnEveryAction => new(Flags.EffectSearchAppliedOnEveryAction);

                /// <summary>
                /// <see cref="Effects.Search"/> is applied at the start of every action phase.
                /// </summary>
                public static Flag AppliedOnActionPhaseStart => new(Flags.EffectSearchAppliedOnActionPhaseStart);
                
                /// <summary>
                /// <see cref="Effects.Search"/> is applied at the end of every action phase.
                /// </summary>
                public static Flag AppliedOnActionPhaseEnd => new(Flags.EffectSearchAppliedOnActionPhaseEnd);
                
                /// <summary>
                /// <see cref="Effects.Search"/> is applied at the start of every planning phase.
                /// </summary>
                public static Flag AppliedOnPlanningPhaseStart => new(Flags.EffectSearchAppliedOnPlanningPhaseStart);
                
                /// <summary>
                /// <see cref="Effects.Search"/> is applied at the end of every planning phase.
                /// </summary>
                public static Flag AppliedOnPlanningPhaseEnd => new(Flags.EffectSearchAppliedOnPlanningPhaseEnd);
                
                /// <summary>
                /// Counteracts any <see cref="Effects"/> added as part of <see cref="Effects.Search"/>
                /// <see cref="Effects.Search.Effects"/> for an <see cref="Entities.Actors.Actor"/> that leaves the
                /// <see cref="Effects.Search.Shape"/>.
                /// </summary>
                public static Flag RemovedOnExit => new(Flags.EffectSearchRemovedOnExit);
                
                /// <summary>
                /// At the start of every planning phase, counteracts any <see cref="Effects"/> added as part of
                /// <see cref="Effects.Search"/> <see cref="Effects.Search.Effects"/>.
                /// </summary>
                public static Flag RemovedOnPlanningPhaseStart => new(Flags.EffectSearchRemovedOnPlanningPhaseStart);
                
                /// <summary>
                /// At the end of every planning phase, counteracts any <see cref="Effects"/> added as part of
                /// <see cref="Effects.Search"/> <see cref="Effects.Search.Effects"/>.
                /// </summary>
                public static Flag RemovedOnPlanningPhaseEnd => new(Flags.EffectSearchRemovedOnPlanningPhaseEnd);
            }
        }

        /// <summary>
        /// Used to filter out <see cref="Entity"/>s. 
        /// </summary>
        public static class Filter // TODO refactor into a Filter type with AND and OR collections to have clear logic
        {
            /// <summary>
            /// Entity is first in the effect chain
            /// </summary>
            public static Flag Origin => new(Flags.FilterOrigin);
            
            /// <summary>
            /// Entity is previous in the effect chain.
            /// </summary>
            public static Flag Source => new(Flags.FilterSource);
            
            /// <summary>
            /// Entity is itself.
            /// </summary>
            public static Flag Self => new(Flags.FilterSelf);

            /// <summary>
            /// Entity is owned by the same player.
            /// </summary>
            public static Flag Player => new(Flags.FilterPlayer);
            
            /// <summary>
            /// Entity is on the same team, but a different player. 
            /// </summary>
            public static Flag Ally => new(Flags.FilterAlly);
            
            /// <summary>
            /// Entity is on an enemy team.
            /// </summary>
            public static Flag Enemy => new(Flags.FilterEnemy);
            
            /// <summary>
            /// Entity is a <see cref="Unit"/>.
            /// </summary>
            public static Flag Unit => new(Flags.FilterUnit);
            
            /// <summary>
            /// Entity is a <see cref="Structure"/>.
            /// </summary>
            public static Flag Structure => new(Flags.FilterStructure);

            /// <summary>
            /// Entity has a <see cref="CombatAttributes"/>.
            /// </summary>
            public static class Attribute
            {
                public static Flag Ranged => new(Flags.FilterAttributeRanged);
            }
            
            /// <summary>
            /// Entity has a specific <see cref="StructureName"/>.
            /// </summary>
            public static class SpecificStructure
            {
                public static Flag Obelisk => new(Flags.FilterSpecificStructureObelisk);
                public static Flag Hut => new(Flags.FilterSpecificStructureHut);
            }

            /// <summary>
            /// Entity has a specific <see cref="UnitName"/>.
            /// </summary>
            public static class SpecificUnit
            {
                public static Flag Horrior => new(Flags.FilterSpecificUnitHorrior);
                public static Flag Cannon => new(Flags.FilterSpecificUnitCannon);
                public static Flag Ballista => new(Flags.FilterSpecificUnitBallista);
                public static Flag Radar => new(Flags.FilterSpecificUnitRadar);
                public static Flag Vessel => new(Flags.FilterSpecificUnitVessel);
            }

            /// <summary>
            /// Entity has a specific <see cref="FeatureName"/>.
            /// </summary>
            public static class SpecificFeature
            {
                public static Flag RadarRedDot => new(Flags.FilterSpecificFeatureRadarRedDot);
            }

            /// <summary>
            /// <see cref="Entity"/> is from a specific <see cref="Faction"/>.
            /// </summary>
            public static class SpecificFaction
            {
                public static Flag Revelators => new(Flags.FilterSpecificFactionRevelators);
                public static Flag Uee => new(Flags.FilterSpecificFactionUee);
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
            ConditionEntityDoesNotExist,
            ConditionEntityExists,
            ConditionTargetDoesNotHaveFullHealth,
            ConditionNoActorsFoundFromEffect,
            ConditionTargetIsLowGround,
            ConditionTargetIsHighGround,
            ConditionTargetIsUnoccupied,
            ConditionTargetIsDifferentTypeThanOrigin,
            ConditionMaskDoesNotExist,
            ConditionMaskExists,
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
            EffectSearchAppliedOnSourceAction,
            EffectSearchAppliedOnEveryAction,
            EffectSearchAppliedOnActionPhaseStart,
            EffectSearchAppliedOnActionPhaseEnd,
            EffectSearchAppliedOnPlanningPhaseStart,
            EffectSearchAppliedOnPlanningPhaseEnd,
            EffectSearchRemovedOnExit,
            EffectSearchRemovedOnPlanningPhaseStart,
            EffectSearchRemovedOnPlanningPhaseEnd,
            FilterOrigin,
            FilterSource,
            FilterSelf,
            FilterPlayer,
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
            FilterSpecificFeatureRadarRedDot, // TODO generalize specific units to not have hardcoded values as well as factions
            FilterSpecificFactionRevelators,
            FilterSpecificFactionUee
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
