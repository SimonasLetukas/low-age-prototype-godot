﻿using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;

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
            public static Flag BehaviourDoesNotExist => new(Flags.ConditionBehaviourDoesNotExist);
            public static Flag BehaviourExists => new(Flags.ConditionBehaviourExists);
            public static Flag TargetDoesNotHaveFullHealth => new(Flags.ConditionTargetDoesNotHaveFullHealth);
            public static Flag NoActorsFoundFromEffect => new(Flags.ConditionNoActorsFoundFromEffect);
            public static Flag TargetIsLowGround => new(Flags.ConditionTargetIsLowGround);
            public static Flag TargetIsHighGround => new(Flags.ConditionTargetIsHighGround);
            public static Flag TargetIsUnoccupied => new(Flags.ConditionTargetIsUnoccupied);
            public static Flag TargetIsDifferentTypeThanOrigin => new(Flags.ConditionTargetIsDifferentTypeThanOrigin);
        }

        public static class Amount
        {
            public static Flag FromMissingHealth => new(Flags.AmountFromMissingHealth);
        }

        public static class Modification
        {
            public static Flag CannotBeHealed => new(Flags.ModificationCannotBeHealed);
            public static Flag AbilitiesDisabled => new(Flags.ModificationAbilitiesDisabled);
            public static Flag ClimbsDown => new(Flags.ModificationClimbsDown);
            public static Flag MovesThroughEnemyUnits => new(Flags.ModificationMovesThroughEnemyUnits);
            public static Flag MustExecuteAttack => new(Flags.ModificationMustExecuteAttack);
            public static Flag CannotAttack => new(Flags.ModificationCannotAttack);
            public static Flag MovementDisabled => new(Flags.ModificationMovementDisabled);
            public static Flag CanAttackAnyTeam => new(Flags.ModificationCanAttackAnyTeam);
            public static Flag IgnoreArmour => new(Flags.ModificationIgnoreArmour);
            public static Flag ProvidesVision => new(Flags.ModificationProvidesVision);
            public static Flag OnlyVisibleToAllies => new(Flags.ModificationOnlyVisibleToAllies);
            public static Flag OnlyVisibleInFogOfWar => new(Flags.ModificationOnlyVisibleInFogOfWar);
            
            /// <summary>
            /// Includes <see cref="MovementDisabled"/>, <see cref="CannotAttack"/>, <see cref="AbilitiesDisabled"/>.
            /// Action turn is always skipped.
            /// </summary>
            public static Flag FullyDisabled => new(Flags.ModificationFullyDisabled);
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
                /// <see cref="Effects.Search.Radius"/>).
                /// </summary>
                public static Flag AppliedOnEnter => new(Flags.EffectSearchAppliedOnEnter);
                
                /// <summary>
                /// Applies <see cref="Effects.Search"/> after <see cref="Durations.EndsAt"/> (at the end of action
                /// for the actor which issued this <see cref="Passive.PeriodicEffect"/>).
                /// </summary>
                public static Flag AppliedOnActorAction => new(Flags.EffectSearchAppliedOnActorAction);
                
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
                /// Counteracts any <see cref="Effects"/> added as part of <see cref="Effects.Search"/>
                /// <see cref="Effects.Search.Effects"/> for an <see cref="Entities.Actors.Actor"/> that leaves the
                /// <see cref="Effects.Search.Radius"/>.
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

        public static class Filter
        {
            public static Flag Origin => new(Flags.FilterOrigin);
            public static Flag Source => new(Flags.FilterSource);
            public static Flag Self => new(Flags.FilterSelf);
            public static Flag Ally => new(Flags.FilterAlly);
            public static Flag Enemy => new(Flags.FilterEnemy);
            public static Flag Unit => new(Flags.FilterUnit);
            public static Flag Structure => new(Flags.FilterStructure);

            public static class Attribute
            {
                public static Flag Ranged => new(Flags.FilterAttributeRanged);
            }
            
            public static class SpecificStructure
            {
                public static Flag Obelisk => new(Flags.FilterSpecificStructureObelisk);
                public static Flag Hut => new(Flags.FilterSpecificStructureHut);
            }

            public static class SpecificUnit
            {
                public static Flag Horrior => new(Flags.FilterSpecificUnitHorrior);
                public static Flag Cannon => new(Flags.FilterSpecificUnitCannon);
                public static Flag Ballista => new(Flags.FilterSpecificUnitBallista);
                public static Flag Radar => new(Flags.FilterSpecificUnitRadar);
                public static Flag Vessel => new(Flags.FilterSpecificUnitVessel);
            }

            public static class SpecificFeature
            {
                public static Flag RadarRedDot => new(Flags.FilterSpecificFeatureRadarRedDot);
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
            EffectSearchAppliedOnActorAction,
            EffectSearchAppliedOnEveryAction,
            EffectSearchAppliedOnActionPhaseStart,
            EffectSearchAppliedOnActionPhaseEnd,
            EffectSearchRemovedOnExit,
            EffectSearchRemovedOnPlanningPhaseStart,
            EffectSearchRemovedOnPlanningPhaseEnd,
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
            FilterSpecificFeatureRadarRedDot,
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}