using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared.Modifications
{
    public class AttackAttribute : ValueObject<AttackAttribute>
    {
        public override string ToString()
        {
            return $"{nameof(AttackAttribute)}.{Value}";
        }

        public static AttackAttribute MaxAmount => new(AttackAttributes.MaxAmount);
        public static AttackAttribute MaxDistance => new(AttackAttributes.MaxDistance);

        private AttackAttribute(AttackAttributes @enum)
        {
            Value = @enum;
        }

        private AttackAttributes Value { get; }

        private enum AttackAttributes
        {
            MaxAmount,
            MinDistance,
            MaxDistance,
            BonusAmount,
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
