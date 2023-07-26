using System.Collections.Generic;

namespace low_age_data.Domain.Shared.Shape
{
    /// <summary>
    /// <see cref="Shape"/> composed of a collection of custom <see cref="Area"/>s.
    /// </summary>
    public class Custom : Shape
    {
        public Custom(
            IList<Area> areas)
            : base($"{nameof(Shape)}.{nameof(Custom)}")
        {
            Areas = areas;
        }
        
        /// <summary>
        /// List of <see cref="Area"/>s, all intersecting to compose a custom <see cref="Shape"/>.
        /// </summary>
        public IList<Area> Areas { get; }
    }
}