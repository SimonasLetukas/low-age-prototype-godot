using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class Research : ValueObject<Research>
    {
        public override string ToString()
        {
            return $"{nameof(Research)}.{Value}";
        }

        public static class Revelators
        {
            public static Research PoisonedSlits => new Research(Researches.PoisonedSlits);
            public static Research SpikedRope => new Research(Researches.SpikedRope);
        }

        private Research(Researches @enum)
        {
            Value = @enum;
        }

        private Researches Value { get; }

        private enum Researches
        {
            PoisonedSlits,
            SpikedRope
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
