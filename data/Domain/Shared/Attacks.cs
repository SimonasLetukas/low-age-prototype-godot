using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class Attacks : ValueObject<Attacks>
    {
        public override string ToString()
        {
            return $"{nameof(Attacks)}.{Value}";
        }

        public static Attacks Melee => new(AttacksEnum.Melee);
        public static Attacks Ranged => new(AttacksEnum.Ranged);

        private Attacks(AttacksEnum @enum)
        {
            Value = @enum;
        }

        private AttacksEnum Value { get; }

        private enum AttacksEnum
        {
            Melee = 0,
            Ranged = 1
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
