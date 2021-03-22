using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Behaviours.Buffs
{
    public class Change : ValueObject<Change>
    {
        public override string ToString()
        {
            return $"{nameof(Change)}.{Value}";
        }

        public static Change Add => new Change(Changes.Add);
        public static Change Remove => new Change(Changes.Remove);
        public static Change Set => new Change(Changes.Set);
        public static Change Multiply => new Change(Changes.Multiply);

        private Change(Changes @enum)
        {
            Value = @enum;
        }

        private Changes Value { get; }

        private enum Changes
        {
            Add = 0,
            Remove = 1,
            Set = 2,
            Multiply = 3
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
