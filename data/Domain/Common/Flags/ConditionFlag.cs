﻿using System.Collections.Generic;
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
        public static ConditionFlag Exists => new ConditionFlag(ConditionFlags.Exists);
        
        /// <summary>
        /// Used in <see cref="MaskCondition"/>, <see cref="EntityCondition"/> or <see cref="BehaviourCondition"/>.
        /// </summary>
        public static ConditionFlag DoesNotExist => new ConditionFlag(ConditionFlags.DoesNotExist);
        
        public static ConditionFlag TargetDoesNotHaveFullHealth => new ConditionFlag(ConditionFlags.TargetDoesNotHaveFullHealth);
        public static ConditionFlag NoActorsFoundFromEffect => new ConditionFlag(ConditionFlags.NoActorsFoundFromEffect);
        public static ConditionFlag TargetIsLowGround => new ConditionFlag(ConditionFlags.TargetIsLowGround);
        public static ConditionFlag TargetIsHighGround => new ConditionFlag(ConditionFlags.TargetIsHighGround);
        public static ConditionFlag TargetIsUnoccupied => new ConditionFlag(ConditionFlags.TargetIsUnoccupied);
        public static ConditionFlag TargetIsDifferentTypeThanOrigin => new ConditionFlag(ConditionFlags.TargetIsDifferentTypeThanOrigin);

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