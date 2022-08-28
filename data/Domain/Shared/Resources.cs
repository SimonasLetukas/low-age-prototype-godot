using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class Resources : ValueObject<Resources>
    {
        public override string ToString()
        {
            return $"{nameof(Resources)}.{Value}";
        }

        public static Resources Scraps => new(ResourcesEnum.Scraps);
        public static Resources Celestium => new(ResourcesEnum.Celestium);

        private Resources(ResourcesEnum @enum)
        {
            Value = @enum;
        }

        private ResourcesEnum Value { get; }

        private enum ResourcesEnum
        {
            Scraps = 0,
            Celestium = 1
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
