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
        
        public static ModificationFlag CannotBeHealed => new ModificationFlag(ModificationFlags.CannotBeHealed);
        public static ModificationFlag AbilitiesDisabled => new ModificationFlag(ModificationFlags.AbilitiesDisabled);
        public static ModificationFlag ClimbsDown => new ModificationFlag(ModificationFlags.ClimbsDown);
        public static ModificationFlag MovesThroughEnemyUnits => new ModificationFlag(ModificationFlags.MovesThroughEnemyUnits);
        public static ModificationFlag MustExecuteAttack => new ModificationFlag(ModificationFlags.MustExecuteAttack);
        public static ModificationFlag CannotAttack => new ModificationFlag(ModificationFlags.CannotAttack);
        public static ModificationFlag MovementDisabled => new ModificationFlag(ModificationFlags.MovementDisabled);
        public static ModificationFlag CanAttackAnyTeam => new ModificationFlag(ModificationFlags.CanAttackAnyTeam);
        public static ModificationFlag IgnoreArmour => new ModificationFlag(ModificationFlags.IgnoreArmour);
        public static ModificationFlag ProvidesVision => new ModificationFlag(ModificationFlags.ProvidesVision);
        public static ModificationFlag OnlyVisibleToAllies => new ModificationFlag(ModificationFlags.OnlyVisibleToAllies);
        public static ModificationFlag OnlyVisibleInFogOfWar => new ModificationFlag(ModificationFlags.OnlyVisibleInFogOfWar);
            
        /// <summary>
        /// Includes <see cref="MovementDisabled"/>, <see cref="CannotAttack"/>, <see cref="AbilitiesDisabled"/>.
        /// Action turn is always skipped.
        /// </summary>
        public static ModificationFlag FullyDisabled => new ModificationFlag(ModificationFlags.FullyDisabled);

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