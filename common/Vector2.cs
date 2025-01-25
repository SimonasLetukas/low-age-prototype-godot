using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeCommon.Extensions;

namespace LowAgeCommon
{
    public static class Vector2IntExtensions
    {
        public static bool IsInBoundsOf(this Vector2<int> vector2, Vector2<int> lowerBounds, Vector2<int> upperBounds) 
            => vector2.X >= lowerBounds.X
               && vector2.Y >= lowerBounds.Y
               && vector2.X < upperBounds.X
               && vector2.Y < upperBounds.Y;
        
        public static bool IsInBoundsOf(this Vector2<int> vector2, Vector2<int> upperBounds) 
            => vector2.X >= 0
               && vector2.Y >= 0
               && vector2.X < upperBounds.X
               && vector2.Y < upperBounds.Y;
        
        public static bool IsDiagonalTo(this Vector2<int> vector2, Vector2<int> point) 
            => vector2.X - vector2.Y == point.X - point.Y 
               || vector2.X + vector2.Y == point.X + point.Y;
        
        public static Area Except(this Vector2<int> size, IEnumerable<Vector2<int>> points)
        {
            var allPoints = new Area(Vector2Int.Zero, size).ToList();
            var remainingPoints = allPoints.Except(points).ToList();
            if (remainingPoints.IsEmpty())
                return new Area(0, 0, 0, 0);
        
            var smallestX = int.MaxValue;
            var smallestY = int.MaxValue;
            var highestX = int.MinValue;
            var highestY = int.MinValue;
            foreach (var point in remainingPoints)
            {
                if (point.X < smallestX)
                    smallestX = point.X;
                if (point.Y < smallestY)
                    smallestY = point.Y;
                if (point.X > highestX)
                    highestX = point.X;
                if (point.Y > highestY)
                    highestY = point.Y;
            }

            return new Area(smallestX, smallestY, highestX - smallestX + 1, highestY - smallestY + 1);
        }
        
        public static IList<Area> ToSquareRects(this IList<Vector2<int>> points)
        {
            var rects = new List<Area>();
            var checkedPoints = points.ToDictionary(point => point, point => false);

            foreach (var point in points)
            {
                if (checkedPoints[point])
                    continue;

                checkedPoints[point] = true;
                var startPoint = new Vector2<int>(point.X, point.Y);
                var size = 1;
                while (NextExtensionExists(checkedPoints, startPoint, size))
                    size++;
            
                rects.Add(new Area(startPoint, new Vector2<int>(size, size)));
            }

            return rects;
        }
        
        private static bool NextExtensionExists(IDictionary<Vector2<int>, bool> points, Vector2<int> startPoint, int depth)
        {
            var validPoints = new List<Vector2<int>>();
            var verticallyValid = true;
            for (var y = 0; y < depth; y++)
            {
                var point = new Vector2<int>(startPoint.X + depth, startPoint.Y + y);
                if (points.ContainsKey(point) && points[point] is false)
                    validPoints.Add(point);
                else
                    verticallyValid = false;
            }
            var horizontallyValid = true;
            for (var x = 0; x < depth; x++)
            {
                var point = new Vector2<int>(startPoint.X + x, startPoint.Y + depth);
                if (points.ContainsKey(point) && points[point] is false)
                    validPoints.Add(point);
                else
                    horizontallyValid = false;
            }

            var finalPoint = new Vector2<int>(startPoint.X + depth, startPoint.Y + depth);
            if (verticallyValid && horizontallyValid && points.ContainsKey(finalPoint) && points[finalPoint] is false)
            {
                validPoints.Add(finalPoint);
                foreach (var point in validPoints)
                {
                    points[point] = true;
                }
                return true;
            }

            return false;
        }
    }
    
    public struct Vector2Int
    {
        public static Vector2<int> One => new Vector2<int>(1, 1);
        public static Vector2<int> Left => new Vector2<int>(-1, 0);
        public static Vector2<int> Right => new Vector2<int>(1, 0);
        public static Vector2<int> Up => new Vector2<int>(0, -1);
        public static Vector2<int> Down => new Vector2<int>(0, 1);
        public static Vector2<int> Zero => new Vector2<int>(0, 0);
        public static Vector2<int> Max => new Vector2<int>(int.MaxValue, int.MaxValue);
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
            if (typeof(T) == typeof(int))
            {
                left.X = (T)(object)((int)(object)left.X - (int)(object)right.X);
                left.Y = (T)(object)((int)(object)left.Y - (int)(object)right.Y);
                return left;
            }

            if (typeof(T) == typeof(double))
            {
                left.X = (T)(object)((double)(object)left.X - (double)(object)right.X);
                left.Y = (T)(object)((double)(object)left.Y - (double)(object)right.Y);
                return left;
            }

            if (typeof(T) == typeof(float))
            {
                left.X = (T)(object)((float)(object)left.X - (float)(object)right.X);
                left.Y = (T)(object)((float)(object)left.Y - (float)(object)right.Y);
                return left;
            }

            if (typeof(T) == typeof(long))
            {
                left.X = (T)(object)((long)(object)left.X - (long)(object)right.X);
                left.Y = (T)(object)((long)(object)left.Y - (long)(object)right.Y);
                return left;
            }

            // Add more numeric types as needed.

            throw new NotSupportedException(
                $"Type {typeof(T)} is not supported for - operator in {nameof(Vector2<T>)}.");
        }

        public static Vector2<T> operator -(Vector2<T> vec)
        {
            if (typeof(T) == typeof(int))
            {
                vec.X = (T)(object)(-(int)(object)vec.X);
                vec.Y = (T)(object)(-(int)(object)vec.Y);
                return vec;
            }

            if (typeof(T) == typeof(double))
            {
                vec.X = (T)(object)(-(double)(object)vec.X);
                vec.Y = (T)(object)(-(double)(object)vec.Y);
                return vec;
            }

            if (typeof(T) == typeof(float))
            {
                vec.X = (T)(object)(-(float)(object)vec.X);
                vec.Y = (T)(object)(-(float)(object)vec.Y);
                return vec;
            }

            if (typeof(T) == typeof(long))
            {
                vec.X = (T)(object)(-(long)(object)vec.X);
                vec.Y = (T)(object)(-(long)(object)vec.Y);
                return vec;
            }

            // Add more numeric types as needed.

            throw new NotSupportedException(
                $"Type {typeof(T)} is not supported for - negative operator in {nameof(Vector2<T>)}.");
        }

        public static Vector2<T> operator *(Vector2<T> vec, int scale) => vec * Convert.ToSingle(scale);

        public static Vector2<T> operator *(int scale, Vector2<T> vec) => vec * Convert.ToSingle(scale);

        public static Vector2<T> operator *(Vector2<T> vec, float scale) => scale * vec;

        public static Vector2<T> operator *(float scale, Vector2<T> vec)
        {
            if (typeof(T) == typeof(int))
            {
                vec.X = (T)(object)Convert.ToInt32((int)(object)vec.X * scale);
                vec.Y = (T)(object)Convert.ToInt32((int)(object)vec.Y * scale);
                return vec;
            }

            if (typeof(T) == typeof(double))
            {
                vec.X = (T)(object)((double)(object)vec.X * scale);
                vec.Y = (T)(object)((double)(object)vec.Y * scale);
                return vec;
            }

            if (typeof(T) == typeof(float))
            {
                vec.X = (T)(object)((float)(object)vec.X * scale);
                vec.Y = (T)(object)((float)(object)vec.Y * scale);
                return vec;
            }

            if (typeof(T) == typeof(long))
            {
                vec.X = (T)(object)Convert.ToInt64((long)(object)vec.X * scale);
                vec.Y = (T)(object)Convert.ToInt64((long)(object)vec.Y * scale);
                return vec;
            }

            // Add more numeric types as needed.

            throw new NotSupportedException(
                $"Type {typeof(T)} is not supported for * scaling operator in {nameof(Vector2<T>)}.");
        }

        public static Vector2<T> operator *(Vector2<T> left, Vector2<T> right)
        {
            if (typeof(T) == typeof(int))
            {
                left.X = (T)(object)((int)(object)left.X * (int)(object)right.X);
                left.Y = (T)(object)((int)(object)left.Y * (int)(object)right.Y);
                return left;
            }

            if (typeof(T) == typeof(double))
            {
                left.X = (T)(object)((double)(object)left.X * (double)(object)right.X);
                left.Y = (T)(object)((double)(object)left.Y * (double)(object)right.Y);
                return left;
            }

            if (typeof(T) == typeof(float))
            {
                left.X = (T)(object)((float)(object)left.X * (float)(object)right.X);
                left.Y = (T)(object)((float)(object)left.Y * (float)(object)right.Y);
                return left;
            }

            if (typeof(T) == typeof(long))
            {
                left.X = (T)(object)((long)(object)left.X * (long)(object)right.X);
                left.Y = (T)(object)((long)(object)left.Y * (long)(object)right.Y);
                return left;
            }

            // Add more numeric types as needed.

            throw new NotSupportedException(
                $"Type {typeof(T)} is not supported for * operator in {nameof(Vector2<T>)}.");
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