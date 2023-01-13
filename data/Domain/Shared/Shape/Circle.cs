using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Shared.Shape
{
    /// <summary>
    /// <see cref="Shape"/> of a circle.
    /// </summary>
    public class Circle : Shape
    {
        public Circle(
            int radius, 
            int? ignoreRadius = null)
            : base($"{nameof(Shape)}.{nameof(Circle)}")
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