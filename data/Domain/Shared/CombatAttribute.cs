using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class CombatAttribute : ValueObject<CombatAttribute>
    {
        public override string ToString()
        {
            return $"{nameof(CombatAttribute)}.{Value}";
        }

        public static CombatAttribute Light => new(CombatAttributes.Light);
        public static CombatAttribute Armoured => new(CombatAttributes.Armoured);
        public static CombatAttribute Giant => new(CombatAttributes.Giant);
        public static CombatAttribute Biological => new(CombatAttributes.Biological);
        public static CombatAttribute Mechanical => new(CombatAttributes.Mechanical);
        public static CombatAttribute Celestial => new(CombatAttributes.Celestial);
        public static CombatAttribute Structure => new(CombatAttributes.Structure);
        public static CombatAttribute Ranged => new(CombatAttributes.Ranged);

        private CombatAttribute(CombatAttributes @enum)
        {
            Value = @enum;
        }

        private CombatAttributes Value { get; }

        private enum CombatAttributes
        {
            Light = 0,
            Armoured = 1,
            Giant = 2,
            Biological = 3,
            Mechanical = 4,
            Celestial = 5,
            Structure = 6,
            Ranged = 7
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
