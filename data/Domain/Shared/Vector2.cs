namespace low_age_data.Domain.Shared
{
    public record Vector2<T>
    {
        public Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public T X { get; }
        public T Y { get; }
    }
}