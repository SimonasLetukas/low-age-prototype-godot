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

        /// <summary>
        /// Applies the same location that was set before in the chain
        /// </summary>
        public static Location Inherited => new(Locations.Inherited);

        /// <summary>
        /// Targets the current actor in the chain
        /// </summary>
        public static Location Self => new(Locations.Self);

        /// <summary>
        /// Targets single actor
        /// </summary>
        public static Location Actor => new(Locations.Actor);

        /// <summary>
        /// Targets point on the ground (tile)
        /// </summary>
        public static Location Point => new(Locations.Point);

        /// <summary>
        /// Targets the previous actor in the chain
        /// </summary>
        public static Location Source => new(Locations.Source);

        /// <summary>
        /// Targets origin - the first actor from which the chain started
        /// </summary>
        public static Location Origin => new(Locations.Origin);

        private Location(Locations @enum)
        {
            Value = @enum;
        }

        private Locations Value { get; }

        private enum Locations
        {
            Inherited = 0,
            Self = 1,
            Actor = 2,
            Point = 3,
            Source = 4,
            Origin = 5
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
