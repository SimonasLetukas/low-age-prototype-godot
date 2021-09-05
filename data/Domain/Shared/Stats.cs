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

        public static Stats Health => new Stats(StatsEnum.Health);
        public static Stats Shields => new Stats(StatsEnum.Shields);
        public static Stats MeleeArmour => new Stats(StatsEnum.MeleeArmour);
        public static Stats RangedArmour => new Stats(StatsEnum.RangedArmour);
        public static Stats Movement => new Stats(StatsEnum.Movement);
        public static Stats Initiative => new Stats(StatsEnum.Initiative);
        public static Stats Vision => new Stats(StatsEnum.Vision);

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
