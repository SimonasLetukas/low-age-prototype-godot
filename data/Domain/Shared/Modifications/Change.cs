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

        public static Change AddMax => new(Changes.AddMax);
        public static Change AddCurrent => new(Changes.AddCurrent);
        public static Change SubtractMax => new(Changes.SubtractMax);
        public static Change SubtractCurrent => new(Changes.SubtractCurrent);
        public static Change SetMax => new(Changes.SetMax);
        public static Change SetCurrent => new(Changes.SetCurrent);
        
        /// <summary>
        /// The result should always be rounded up by using a ceiling function.
        /// </summary>
        public static Change MultiplyMax => new(Changes.MultiplyMax);
        public static Change MultiplyCurrent => new(Changes.MultiplyCurrent);

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
