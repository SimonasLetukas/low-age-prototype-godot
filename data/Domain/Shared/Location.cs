using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class Location : ValueObject<Location>
    {
        public override string ToString()
        {
            return $"{nameof(Location)}.{Value}";
        }

        public static Location Inherited => new Location(Locations.Inherited);
        public static Location Self => new Location(Locations.Self);
        public static Location Actor => new Location(Locations.Actor);

        private Location(Locations @enum)
        {
            Value = @enum;
        }

        private Locations Value { get; }

        private enum Locations
        {
            Inherited = 0, // Applies the same location that was set before in the chain
            Self = 1, // Targets self actor
            Actor = 2, // Targets single actor
            Point = 3, // Targets point
            Source = 4, // Targets source - the previous actor in the chain
            Origin = 5 // Targets origin - the first actor from which the chain started
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
