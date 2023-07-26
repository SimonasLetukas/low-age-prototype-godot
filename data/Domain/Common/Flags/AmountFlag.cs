using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared.Flags
{
    public class AmountFlag : ValueObject<AmountFlag>
    {
        public override string ToString()
        {
            return $"{nameof(AmountFlag)}.{Value}";
        }
        
        public static AmountFlag FromMissingHealth => new AmountFlag(AmountFlags.FromMissingHealth);

        private AmountFlag(AmountFlags @enum)
        {
            Value = @enum;
        }

        private AmountFlags Value { get; }

        private enum AmountFlags
        {
            FromMissingHealth
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}