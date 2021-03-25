using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class Shape : ValueObject<Shape>
    {
        public override string ToString()
        {
            return $"{nameof(Shape)}.{Value}";
        }

        public static Shape Circle => new Shape(Shapes.Circle);
        public static Shape Map => new Shape(Shapes.Map);

        private Shape(Shapes @enum)
        {
            Value = @enum;
        }

        private Shapes Value { get; }

        private enum Shapes
        {
            Circle,
            Map
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
