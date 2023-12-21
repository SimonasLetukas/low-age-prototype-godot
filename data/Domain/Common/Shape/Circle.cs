using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Common.Shape
{
    /// <summary>
    /// <see cref="IShape"/> of a circle.
    /// </summary>
    public class Circle : IShape
    {
        public Circle(int radius, int? ignoreRadius = null)
        {
            Radius = radius;
            IgnoreRadius = ignoreRadius ?? -1;
        }
        
        /// <summary>
        /// Radius of the circle, starting from 0 (only the center tile).
        /// </summary>
        public int Radius { get; }
        
        /// <summary>
        /// -1 to not ignore anything (default value). 0 to ignore center. 1 to ignore all adjacent
        /// <see cref="Entity"/>s, etc.
        /// </summary>
        public int IgnoreRadius { get; }
    }
}