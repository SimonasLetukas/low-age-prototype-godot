using System;

namespace low_age_data.Domain.Common
{
    [Serializable]
    public struct Vector2<T> : IEquatable<Vector2<T>> where T : struct
    {
        public Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public T X { get; }
        public T Y { get; }
        
        public override string ToString()
        {
            return $"({(object)X}, {(object)Y})";
        }

        public bool Equals(Vector2<T> other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector2<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }
    }
}