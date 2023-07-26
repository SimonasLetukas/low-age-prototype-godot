namespace low_age_data.Domain.Shared.Shape
{
    /// <summary>
    /// <see cref="Shape"/> of the whole map.
    /// </summary>
    public class Map : Shape
    {
        public Map() : base($"{nameof(Shape)}.{nameof(Map)}")
        {
        }
    }
}