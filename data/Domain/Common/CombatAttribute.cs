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

        public static CombatAttribute Light => new CombatAttribute(CombatAttributes.Light);
        public static CombatAttribute Armoured => new CombatAttribute(CombatAttributes.Armoured);
        public static CombatAttribute Giant => new CombatAttribute(CombatAttributes.Giant);
        public static CombatAttribute Biological => new CombatAttribute(CombatAttributes.Biological);
        public static CombatAttribute Mechanical => new CombatAttribute(CombatAttributes.Mechanical);
        public static CombatAttribute Celestial => new CombatAttribute(CombatAttributes.Celestial);
        public static CombatAttribute Structure => new CombatAttribute(CombatAttributes.Structure);
        public static CombatAttribute Ranged => new CombatAttribute(CombatAttributes.Ranged);

        private CombatAttribute(CombatAttributes @enum)
        {
            Value = @enum;
        }

        private CombatAttributes Value { get; }

        private enum CombatAttributes
        {
            Light,
            Armoured,
            Giant,
            Biological,
            Mechanical,
            Celestial,
            Structure,
            Ranged
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
