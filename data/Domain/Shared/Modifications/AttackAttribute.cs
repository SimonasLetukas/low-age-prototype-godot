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

        public static AttackAttribute MaxAmount => new AttackAttribute(AttackAttributes.MaxAmount);

        private AttackAttribute(AttackAttributes @enum)
        {
            Value = @enum;
        }

        private AttackAttributes Value { get; }

        private enum AttackAttributes
        {
            MaxAmount = 0,
            MinimumDistance = 1,
            MaximumDistance = 2,
            BonusAmount = 3,
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
