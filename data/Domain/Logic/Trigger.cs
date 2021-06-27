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

        public static Trigger OriginIsDestroyed => new Trigger(Triggers.OriginIsDestroyed);
        public static Trigger SourceIsDestroyed => new Trigger(Triggers.SourceIsDestroyed);
        public static Trigger EntityIsAboutToMove => new Trigger(Triggers.EntityIsAboutToMove);

        private Trigger(Triggers @enum)
        {
            Value = @enum;
        }

        private Triggers Value { get; }

        private enum Triggers
        {
            OriginIsDestroyed,
            SourceIsDestroyed,
            EntityIsAboutToMove
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
