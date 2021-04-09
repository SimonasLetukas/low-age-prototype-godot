using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class DamageType : ValueObject<DamageType>
    {
        public override string ToString()
        {
            return $"{nameof(DamageType)}.{Value}";
        }

        public static DamageType Melee => new DamageType(DamageTypes.Melee);
        public static DamageType Ranged => new DamageType(DamageTypes.Ranged);
        public static DamageType Pure => new DamageType(DamageTypes.Pure);
        public static DamageType CurrentMelee => new DamageType(DamageTypes.CurrentMelee);
        public static DamageType CurrentRanged => new DamageType(DamageTypes.CurrentRanged);

        private DamageType(DamageTypes @enum)
        {
            Value = @enum;
        }

        private DamageTypes Value { get; }

        private enum DamageTypes
        {
            Melee, // Deals damage subtracted by melee armour of the target
            Ranged, // Deals damage subtracted by ranged armour of the target
            Pure, // Deals damage directly to health
            CurrentMelee, // Any damage amount is overwritten by the source's current melee attack damage
            CurrentRanged, // Any damage amount is overwritten by the source's current ranged attack damage
            TargetMelee, // Any damage amount is overwritten by the target's current melee attack damage
            TargetRanged // Any damage amount is overwritten by the target's current ranged attack damage
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
