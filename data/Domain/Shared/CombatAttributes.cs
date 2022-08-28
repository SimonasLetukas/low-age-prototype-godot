using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class CombatAttributes : ValueObject<CombatAttributes>
    {
        public override string ToString()
        {
            return $"{nameof(CombatAttributes)}.{Value}";
        }

        public static CombatAttributes Light => new(CombatAttributesEnum.Light);
        public static CombatAttributes Armoured => new(CombatAttributesEnum.Armoured);
        public static CombatAttributes Giant => new(CombatAttributesEnum.Giant);
        public static CombatAttributes Biological => new(CombatAttributesEnum.Biological);
        public static CombatAttributes Mechanical => new(CombatAttributesEnum.Mechanical);
        public static CombatAttributes Celestial => new(CombatAttributesEnum.Celestial);
        public static CombatAttributes Structure => new(CombatAttributesEnum.Structure);
        public static CombatAttributes Ranged => new(CombatAttributesEnum.Ranged);

        private CombatAttributes(CombatAttributesEnum @enum)
        {
            Value = @enum;
        }

        private CombatAttributesEnum Value { get; }

        private enum CombatAttributesEnum
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
