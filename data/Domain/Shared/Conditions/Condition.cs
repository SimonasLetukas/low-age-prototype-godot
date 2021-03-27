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
            public static Condition DoesNotAlreadyExist =>
                new Condition(Conditions.BehaviourToApplyDoesNotAlreadyExist);
        }

        public static class Target
        {
            public static Condition DoesNotHaveFullHealth =>
                new Condition(Conditions.TargetDoesNotHaveFullHealth);
        }

        public static class Effect
        {
            public static class Chain
            {
                public static Condition OriginIsDead => new Condition(Conditions.EffectChainOriginIsDead);
            }
        }

        private Condition(Conditions @enum)
        {
            Value = @enum;
        }

        private Conditions Value { get; }

        private enum Conditions
        {
            BehaviourToApplyDoesNotAlreadyExist,
            TargetDoesNotHaveFullHealth,
            EffectChainOriginIsDead
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
