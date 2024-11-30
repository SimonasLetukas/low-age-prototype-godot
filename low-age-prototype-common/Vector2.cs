using System;

namespace low_age_prototype_common
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
        
        public static Vector2<T> operator +(Vector2<T> left, Vector2<T> right)
        {
            return new Vector2<T>(
                Add(left.X, right.X),
                Add(left.Y, right.Y)
            );
        }

        private static T Add(T a, T b)
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)((int)(object)a + (int)(object)b);
            }
            if (typeof(T) == typeof(double))
            {
                return (T)(object)((double)(object)a + (double)(object)b);
            }
            if (typeof(T) == typeof(float))
            {
                return (T)(object)((float)(object)a + (float)(object)b);
            }
            if (typeof(T) == typeof(long))
            {
                return (T)(object)((long)(object)a + (long)(object)b);
            }
            // Add more numeric types as needed.

            throw new NotSupportedException($"Type {typeof(T)} is not supported for addition in {nameof(Vector2<T>)}.");
        }
    }
}