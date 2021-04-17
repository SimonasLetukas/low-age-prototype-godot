using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Logic
{
    public class Condition : ValueObject<Condition>
    {
        public override string ToString()
        {
            return $"{nameof(Condition)}.{Value}";
        }

        public static Condition BehaviourToApplyDoesNotAlreadyExist => new Condition(Conditions.BehaviourToApplyDoesNotAlreadyExist);
        public static Condition TargetDoesNotHaveFullHealth => new Condition(Conditions.TargetDoesNotHaveFullHealth);
        public static Condition NoActorsFoundFromEffect => new Condition(Conditions.NoActorsFoundFromEffect);
        public static Condition TargetIsHighGround => new Condition(Conditions.TargetIsHighGround);
        public static Condition TargetIsUnoccupied => new Condition(Conditions.TargetIsUnoccupied);

        private Condition(Conditions @enum)
        {
            Value = @enum;
        }

        private Conditions Value { get; }

        private enum Conditions
        {
            BehaviourToApplyDoesNotAlreadyExist,
            TargetDoesNotHaveFullHealth,
            NoActorsFoundFromEffect,
            TargetIsHighGround,
            TargetIsUnoccupied
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
