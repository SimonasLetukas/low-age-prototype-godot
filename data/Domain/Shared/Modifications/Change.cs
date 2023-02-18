using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared.Modifications
{
    public class Change : ValueObject<Change>
    {
        public override string ToString()
        {
            return $"{nameof(Change)}.{Value}";
        }

        public static Change AddMax => new Change(Changes.AddMax);
        public static Change AddCurrent => new Change(Changes.AddCurrent);
        public static Change SubtractMax => new Change(Changes.SubtractMax);
        public static Change SubtractCurrent => new Change(Changes.SubtractCurrent);
        public static Change SetMax => new Change(Changes.SetMax);
        public static Change SetCurrent => new Change(Changes.SetCurrent);
        
        /// <summary>
        /// The result should always be rounded up by using a ceiling function.
        /// </summary>
        public static Change MultiplyMax => new Change(Changes.MultiplyMax);
        public static Change MultiplyCurrent => new Change(Changes.MultiplyCurrent);

        private Change(Changes @enum)
        {
            Value = @enum;
        }

        private Changes Value { get; }

        private enum Changes
        {
            AddMax,
            AddCurrent,
            SubtractMax,
            SubtractCurrent,
            SetMax,
            SetCurrent,
            MultiplyMax,
            MultiplyCurrent
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
