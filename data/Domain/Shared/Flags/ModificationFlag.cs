using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared.Flags
{
    public class ModificationFlag : ValueObject<ModificationFlag>
    {
        public override string ToString()
        {
            return $"{nameof(ModificationFlag)}.{Value}";
        }
        
        public static ModificationFlag CannotBeHealed => new(ModificationFlags.CannotBeHealed);
        public static ModificationFlag AbilitiesDisabled => new(ModificationFlags.AbilitiesDisabled);
        public static ModificationFlag ClimbsDown => new(ModificationFlags.ClimbsDown);
        public static ModificationFlag MovesThroughEnemyUnits => new(ModificationFlags.MovesThroughEnemyUnits);
        public static ModificationFlag MustExecuteAttack => new(ModificationFlags.MustExecuteAttack);
        public static ModificationFlag CannotAttack => new(ModificationFlags.CannotAttack);
        public static ModificationFlag MovementDisabled => new(ModificationFlags.MovementDisabled);
        public static ModificationFlag CanAttackAnyTeam => new(ModificationFlags.CanAttackAnyTeam);
        public static ModificationFlag IgnoreArmour => new(ModificationFlags.IgnoreArmour);
        public static ModificationFlag ProvidesVision => new(ModificationFlags.ProvidesVision);
        public static ModificationFlag OnlyVisibleToAllies => new(ModificationFlags.OnlyVisibleToAllies);
        public static ModificationFlag OnlyVisibleInFogOfWar => new(ModificationFlags.OnlyVisibleInFogOfWar);
            
        /// <summary>
        /// Includes <see cref="MovementDisabled"/>, <see cref="CannotAttack"/>, <see cref="AbilitiesDisabled"/>.
        /// Action turn is always skipped.
        /// </summary>
        public static ModificationFlag FullyDisabled => new(ModificationFlags.FullyDisabled);

        private ModificationFlag(ModificationFlags @enum)
        {
            Value = @enum;
        }

        private ModificationFlags Value { get; }

        private enum ModificationFlags
        {
            CannotBeHealed,
            AbilitiesDisabled,
            ClimbsDown,
            MovesThroughEnemyUnits,
            MustExecuteAttack,
            CannotAttack,
            MovementDisabled,
            CanAttackAnyTeam,
            IgnoreArmour,
            ProvidesVision,
            OnlyVisibleToAllies,
            OnlyVisibleInFogOfWar,
            FullyDisabled
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}