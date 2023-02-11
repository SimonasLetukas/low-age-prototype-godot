using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Shared.Flags
{
    public class ConditionFlag : ValueObject<ConditionFlag>
    {
        public override string ToString()
        {
            return $"{nameof(ConditionFlag)}.{Value}";
        }
        
        /// <summary>
        /// Used in <see cref="MaskCondition"/>, <see cref="EntityCondition"/> or <see cref="BehaviourCondition"/>.
        /// </summary>
        public static ConditionFlag Exists => new(ConditionFlags.Exists);
        
        /// <summary>
        /// Used in <see cref="MaskCondition"/>, <see cref="EntityCondition"/> or <see cref="BehaviourCondition"/>.
        /// </summary>
        public static ConditionFlag DoesNotExist => new(ConditionFlags.DoesNotExist);
        
        public static ConditionFlag TargetDoesNotHaveFullHealth => new(ConditionFlags.TargetDoesNotHaveFullHealth);
        public static ConditionFlag NoActorsFoundFromEffect => new(ConditionFlags.NoActorsFoundFromEffect);
        public static ConditionFlag TargetIsLowGround => new(ConditionFlags.TargetIsLowGround);
        public static ConditionFlag TargetIsHighGround => new(ConditionFlags.TargetIsHighGround);
        public static ConditionFlag TargetIsUnoccupied => new(ConditionFlags.TargetIsUnoccupied);
        public static ConditionFlag TargetIsDifferentTypeThanOrigin => new(ConditionFlags.TargetIsDifferentTypeThanOrigin);

        private ConditionFlag(ConditionFlags @enum)
        {
            Value = @enum;
        }

        private ConditionFlags Value { get; }

        private enum ConditionFlags
        {
            Exists,
            DoesNotExist,
            TargetDoesNotHaveFullHealth,
            NoActorsFoundFromEffect,
            TargetIsLowGround,
            TargetIsHighGround,
            TargetIsUnoccupied,
            TargetIsDifferentTypeThanOrigin
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}