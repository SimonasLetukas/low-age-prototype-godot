using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Logic
{
    public class Trigger : ValueObject<Trigger>
    {
        public override string ToString()
        {
            return $"{nameof(Trigger)}.{Value}";
        }

        public static Trigger OriginIsDead => new Trigger(Triggers.OriginIsDead); 

        private Trigger(Triggers @enum)
        {
            Value = @enum;
        }

        private Triggers Value { get; }

        private enum Triggers
        {
            OriginIsDead
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
