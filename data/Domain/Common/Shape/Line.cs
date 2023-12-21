using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Common.Shape
{
    /// <summary>
    /// <see cref="IShape"/> of a line, width of the source <see cref="Entity"/>.
    /// </summary>
    public class Line : IShape
    {
        public Line(int length, int? ignoreLength = null) 
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