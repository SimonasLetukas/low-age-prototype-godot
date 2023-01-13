using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Shared.Shape
{
    /// <summary>
    /// <see cref="Shape"/> of a line, width of the source <see cref="Entity"/>.
    /// </summary>
    public class Line : Shape
    {
        public Line(
            int length,
            int? ignoreLength = null) 
            : base($"{nameof(Shape)}.{nameof(Line)}")
        {
            Length = length;
            IgnoreLength = ignoreLength ?? -1;
        }
        
        /// <summary>
        /// Length of the line, starting from 0 (targeting inside of the <see cref="Entity"/>).
        /// </summary>
        public int Length { get; }
        
        /// <summary>
        /// -1 to not ignore anything (default value). 0 to ignore inside of the <see cref="Entity"/>. 1 and above
        /// to ignore tiles outside of the <see cref="Entity"/>.
        /// </summary>
        public int IgnoreLength { get; }
    }
}