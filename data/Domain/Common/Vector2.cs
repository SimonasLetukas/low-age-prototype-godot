namespace low_age_data.Domain.Shared
{
    public struct Vector2<T> where T : struct
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