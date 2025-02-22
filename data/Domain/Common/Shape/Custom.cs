using LowAgeCommon;

namespace LowAgeData.Domain.Common.Shape
{
    /// <summary>
    /// <see cref="IShape"/> composed of a collection of custom <see cref="Area"/>s.
    /// </summary>
    public class Custom : IShape
    {
        public Custom(IList<Area> areas)
        {
            Areas = areas;
        }
        
        /// <summary>
        /// List of <see cref="Area"/>s, all intersecting to compose a custom <see cref="IShape"/>.
        /// </summary>
        public IList<Area> Areas { get; }
    }
}