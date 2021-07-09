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
                public static Flag AppliedOnAction => new Flag(Flags.EffectSearchAppliedOnAction);
                public static Flag RemovedOnExit => new Flag(Flags.EffectSearchRemovedOnExit);
            }
        }

        public static class Filter
        {
            public static Flag Self => new Flag(Flags.FilterSelf);
            public static Flag Ally => new Flag(Flags.FilterAlly);
            public static Flag Enemy => new Flag(Flags.FilterEnemy);
            public static Flag Unit => new Flag(Flags.FilterUnit);
            public static Flag Structure => new Flag(Flags.FilterStructure);
        }

        public static class Structure
        {
            public static Flag Obelisk => new Flag(Flags.StructureObelisk);
            public static Flag Hut => new Flag(Flags.StructureHut);
        }

        public static class Unit
        {
            public static Flag Horrior => new Flag(Flags.UnitHorrior);
        }

        private Flag(Flags @enum)
        {
            Value = @enum;
        }

        private Flags Value { get; }

        private enum Flags
        {
            AmountFromMissingHealth,
            ModificationCannotBeHealed,
            ModificationAbilitiesDisabled,
            ModificationClimbsDown,
            ModificationMovesThroughEnemyUnits,
            ModificationMustExecuteAttack,
            ModificationCannotAttack,
            ModificationMovementDisabled,
            ModificationCanAttackAnyTeam,
            EffectModifyPlayerGameLost,
            EffectSearchAppliedOnEnter,
            EffectSearchAppliedOnAction,
            EffectSearchRemovedOnExit,
            FilterSelf,
            FilterAlly,
            FilterEnemy,
            FilterUnit,
            FilterStructure,
            StructureObelisk,
            StructureHut,
            UnitHorrior
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
