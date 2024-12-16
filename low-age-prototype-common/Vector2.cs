using System;

namespace low_age_prototype_common
{
    public struct Vector2Int
    {
        public static Vector2<int> One => new Vector2<int>(1, 1);
        public static Vector2<int> Left => new Vector2<int>(-1, 0);
        public static Vector2<int> Right => new Vector2<int>(1, 0);
        public static Vector2<int> Up => new Vector2<int>(0, -1);
        public static Vector2<int> Down => new Vector2<int>(0, 1);
        public static Vector2<int> Zero => new Vector2<int>(0, 0);
    }

    [Serializable]
    public struct Vector2<T> : IEquatable<Vector2<T>> where T : struct
    {
        public Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public T X { get; private set; }
        public T Y { get; private set; }

        public bool IsInBoundsOf(Vector2<T> lowerBounds, Vector2<T> upperBounds)
            => (float)(object)X >= (float)(object)lowerBounds.X
               && (float)(object)Y >= (float)(object)lowerBounds.Y
               && (float)(object)X < (float)(object)upperBounds.X
               && (float)(object)Y < (float)(object)upperBounds.Y;

        public bool IsInBoundsOf(Vector2<T> upperBounds)
            => (float)(object)X >= 0
               && (float)(object)Y >= 0
               && (float)(object)X < (float)(object)upperBounds.X
               && (float)(object)Y < (float)(object)upperBounds.Y;

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

        public static Vector2<T> operator -(Vector2<T> left, Vector2<T> right)
        {
            left.X = (T)(object)((float)(object)left.X - (float)(object)right.X);
            left.Y = (T)(object)((float)(object)left.Y - (float)(object)right.Y);
            return left;
        }

        public static Vector2<T> operator -(Vector2<T> vec)
        {
            vec.X = (T)(object)(-(float)(object)vec.X);
            vec.Y = (T)(object)(-(float)(object)vec.Y);
            return vec;
        }

        public static Vector2<T> operator *(Vector2<T> vec, float scale)
        {
            vec.X = (T)(object)((float)(object)vec.X * scale);
            vec.Y = (T)(object)((float)(object)vec.Y * scale);
            return vec;
        }

        public static Vector2<T> operator *(float scale, Vector2<T> vec)
        {
            vec.X = (T)(object)((float)(object)vec.X * scale);
            vec.Y = (T)(object)((float)(object)vec.Y * scale);
            return vec;
        }

        public static Vector2<T> operator *(Vector2<T> left, Vector2<T> right)
        {
            left.X = (T)(object)((float)(object)left.X * (float)(object)right.X);
            left.Y = (T)(object)((float)(object)left.Y * (float)(object)right.Y);
            return left;
        }

        public static bool operator ==(Vector2<T> left, Vector2<T> right) => left.Equals(right);

        public static bool operator !=(Vector2<T> left, Vector2<T> right) => !left.Equals(right);

        public static bool operator <(Vector2<T> left, Vector2<T> right)
        {
            if (typeof(T) == typeof(int))
            {
                return (int)(object)left.X == (int)(object)right.X
                    ? (int)(object)left.Y < (int)(object)right.Y
                    : (int)(object)left.X < (int)(object)right.X;
            }

            if (typeof(T) == typeof(double))
            {
                return (double)(object)left.X == (double)(object)right.X
                    ? (double)(object)left.Y < (double)(object)right.Y
                    : (double)(object)left.X < (double)(object)right.X;
            }

            if (typeof(T) == typeof(float))
            {
                return (float)(object)left.X == (float)(object)right.X
                    ? (float)(object)left.Y < (float)(object)right.Y
                    : (float)(object)left.X < (float)(object)right.X;
            }

            if (typeof(T) == typeof(long))
            {
                return (long)(object)left.X == (long)(object)right.X
                    ? (long)(object)left.Y < (long)(object)right.Y
                    : (long)(object)left.X < (long)(object)right.X;
            }

            // Add more numeric types as needed.

            throw new NotSupportedException(
                $"Type {typeof(T)} is not supported for < operator in {nameof(Vector2<T>)}.");
        }

        public static bool operator >(Vector2<T> left, Vector2<T> right)
        {
            if (typeof(T) == typeof(int))
            {
                return (int)(object)left.X == (int)(object)right.X
                    ? (int)(object)left.Y > (int)(object)right.Y
                    : (int)(object)left.X > (int)(object)right.X;
            }

            if (typeof(T) == typeof(double))
            {
                return (double)(object)left.X == (double)(object)right.X
                    ? (double)(object)left.Y > (double)(object)right.Y
                    : (double)(object)left.X > (double)(object)right.X;
            }

            if (typeof(T) == typeof(float))
            {
                return (float)(object)left.X == (float)(object)right.X
                    ? (float)(object)left.Y > (float)(object)right.Y
                    : (float)(object)left.X > (float)(object)right.X;
            }

            if (typeof(T) == typeof(long))
            {
                return (long)(object)left.X == (long)(object)right.X
                    ? (long)(object)left.Y > (long)(object)right.Y
                    : (long)(object)left.X > (long)(object)right.X;
            }

            // Add more numeric types as needed.

            throw new NotSupportedException(
                $"Type {typeof(T)} is not supported for > operator in {nameof(Vector2<T>)}.");
        }

        public static bool operator <=(Vector2<T> left, Vector2<T> right)
        {
            if (typeof(T) == typeof(int))
            {
                return (int)(object)left.X == (int)(object)right.X
                    ? (int)(object)left.Y <= (int)(object)right.Y
                    : (int)(object)left.X < (int)(object)right.X;
            }

            if (typeof(T) == typeof(double))
            {
                return (double)(object)left.X == (double)(object)right.X
                    ? (double)(object)left.Y <= (double)(object)right.Y
                    : (double)(object)left.X < (double)(object)right.X;
            }

            if (typeof(T) == typeof(float))
            {
                return (float)(object)left.X == (float)(object)right.X
                    ? (float)(object)left.Y <= (float)(object)right.Y
                    : (float)(object)left.X < (float)(object)right.X;
            }

            if (typeof(T) == typeof(long))
            {
                return (long)(object)left.X == (long)(object)right.X
                    ? (long)(object)left.Y <= (long)(object)right.Y
                    : (long)(object)left.X < (long)(object)right.X;
            }

            // Add more numeric types as needed.

            throw new NotSupportedException(
                $"Type {typeof(T)} is not supported for <= operator in {nameof(Vector2<T>)}.");
        }

        public static bool operator >=(Vector2<T> left, Vector2<T> right)
        {
            if (typeof(T) == typeof(int))
            {
                return (int)(object)left.X == (int)(object)right.X
                    ? (int)(object)left.Y >= (int)(object)right.Y
                    : (int)(object)left.X > (int)(object)right.X;
            }

            if (typeof(T) == typeof(double))
            {
                return (double)(object)left.X == (double)(object)right.X
                    ? (double)(object)left.Y >= (double)(object)right.Y
                    : (double)(object)left.X > (double)(object)right.X;
            }

            if (typeof(T) == typeof(float))
            {
                return (float)(object)left.X == (float)(object)right.X
                    ? (float)(object)left.Y >= (float)(object)right.Y
                    : (float)(object)left.X > (float)(object)right.X;
            }

            if (typeof(T) == typeof(long))
            {
                return (long)(object)left.X == (long)(object)right.X
                    ? (long)(object)left.Y >= (long)(object)right.Y
                    : (long)(object)left.X > (long)(object)right.X;
            }

            // Add more numeric types as needed.

            throw new NotSupportedException(
                $"Type {typeof(T)} is not supported for >= operator in {nameof(Vector2<T>)}.");
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