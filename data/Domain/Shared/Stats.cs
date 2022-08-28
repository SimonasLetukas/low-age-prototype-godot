using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class Stats : ValueObject<Stats>
    {
        public override string ToString()
        {
            return $"{nameof(Stats)}.{Value}";
        }

        public static Stats Health => new(StatsEnum.Health);
        public static Stats Shields => new(StatsEnum.Shields);
        public static Stats MeleeArmour => new(StatsEnum.MeleeArmour);
        public static Stats RangedArmour => new(StatsEnum.RangedArmour);
        public static Stats Movement => new(StatsEnum.Movement);
        public static Stats Initiative => new(StatsEnum.Initiative);
        public static Stats Vision => new(StatsEnum.Vision);

        private Stats(StatsEnum @enum)
        {
            Value = @enum;
        }

        private StatsEnum Value { get; }

        private enum StatsEnum
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
