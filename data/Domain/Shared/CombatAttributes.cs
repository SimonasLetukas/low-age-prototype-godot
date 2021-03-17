using System.Collections.Generic;

namespace low_age_data.Domain.Shared
{
    public class CombatAttributes : ValueObject<CombatAttributes>
    {
        public override string ToString()
        {
            return $"{nameof(CombatAttributes)}.{Value}";
        }

        public static CombatAttributes Light => new CombatAttributes(CombatAttributesEnum.Light);
        public static CombatAttributes Armoured => new CombatAttributes(CombatAttributesEnum.Armoured);
        public static CombatAttributes Giant => new CombatAttributes(CombatAttributesEnum.Giant);
        public static CombatAttributes Biological => new CombatAttributes(CombatAttributesEnum.Biological);
        public static CombatAttributes Mechanical => new CombatAttributes(CombatAttributesEnum.Mechanical);
        public static CombatAttributes Celestial => new CombatAttributes(CombatAttributesEnum.Celestial);
        public static CombatAttributes Structure => new CombatAttributes(CombatAttributesEnum.Structure);
        public static CombatAttributes Ranged => new CombatAttributes(CombatAttributesEnum.Ranged);

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
