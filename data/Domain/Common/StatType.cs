using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class StatType : ValueObject<StatType>
    {
        public override string ToString()
        {
            return $"{nameof(StatType)}.{Value}";
        }

        public static StatType Health => new StatType(StatTypeEnum.Health);
        public static StatType Shields => new StatType(StatTypeEnum.Shields);
        public static StatType MeleeArmour => new StatType(StatTypeEnum.MeleeArmour);
        public static StatType RangedArmour => new StatType(StatTypeEnum.RangedArmour);
        public static StatType Movement => new StatType(StatTypeEnum.Movement);
        public static StatType Initiative => new StatType(StatTypeEnum.Initiative);
        public static StatType Vision => new StatType(StatTypeEnum.Vision);

        private StatType(StatTypeEnum @enum)
        {
            Value = @enum;
        }

        private StatTypeEnum Value { get; }

        private enum StatTypeEnum
        {
            Health,
            Shields,
            MeleeArmour,
            RangedArmour,
            Movement,
            Initiative,
            Vision
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
