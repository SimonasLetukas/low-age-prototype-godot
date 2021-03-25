using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared.Conditions
{
    public class Condition : ValueObject<Condition>
    {
        public override string ToString()
        {
            return $"{nameof(Condition)}.{Value}";
        }

        public static class Behaviour
        {
            public static Condition BehaviourToApplyDoesNotAlreadyExist =>
                new Condition(Conditions.BehaviourToApplyDoesNotAlreadyExist);
        }

        private Condition(Conditions @enum)
        {
            Value = @enum;
        }

        private Conditions Value { get; }

        private enum Conditions
        {
            BehaviourToApplyDoesNotAlreadyExist
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
