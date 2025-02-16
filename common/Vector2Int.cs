using LowAgeCommon.Extensions;

namespace LowAgeCommon;

public static class Vector2IntExtensions
{
    public static bool IsInBoundsOf(this Vector2Int vector2, Vector2Int lowerBounds, Vector2Int upperBounds)
        => vector2.X >= lowerBounds.X
           && vector2.Y >= lowerBounds.Y
           && vector2.X < upperBounds.X
           && vector2.Y < upperBounds.Y;

    public static bool IsInBoundsOf(this Vector2Int vector2, Vector2Int upperBounds)
        => vector2 is { X: >= 0, Y: >= 0 }
           && vector2.X < upperBounds.X
           && vector2.Y < upperBounds.Y;

    public static Vector2Int ClampBetween(this Vector2Int vector2, Vector2Int min, Vector2Int max)
    {
        vector2.X = Math.Clamp(vector2.X, min.X, max.X);
        vector2.Y = Math.Clamp(vector2.Y, min.Y, max.Y);
        return vector2;
    }

    public static bool IsDiagonalTo(this Vector2Int vector2, Vector2Int point)
        => vector2.X - vector2.Y == point.X - point.Y
           || vector2.X + vector2.Y == point.X + point.Y;

    public static Area Except(this Vector2Int size, IEnumerable<Vector2Int> points)
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

    public static IList<Area> ToSquareRects(this IList<Vector2Int> points)
    {
        var rects = new List<Area>();
        var checkedPoints = points.ToDictionary(point => point, _ => false);

        foreach (var point in points)
        {
            if (checkedPoints[point])
                continue;

            checkedPoints[point] = true;
            var startPoint = new Vector2Int(point.X, point.Y);
            var size = 1;
            while (NextExtensionExists(checkedPoints, startPoint, size))
                size++;

            rects.Add(new Area(startPoint, new Vector2Int(size, size)));
        }

        return rects;
    }

    private static bool NextExtensionExists(Dictionary<Vector2Int, bool> points, Vector2Int startPoint, int depth)
    {
        var validPoints = new List<Vector2Int>();
        var verticallyValid = true;
        for (var y = 0; y < depth; y++)
        {
            var point = new Vector2Int(startPoint.X + depth, startPoint.Y + y);
            if (points.TryGetValue(point, out var value) && value is false)
                validPoints.Add(point);
            else
                verticallyValid = false;
        }

        var horizontallyValid = true;
        for (var x = 0; x < depth; x++)
        {
            var point = new Vector2Int(startPoint.X + x, startPoint.Y + depth);
            if (points.TryGetValue(point, out var value) && value is false)
                validPoints.Add(point);
            else
                horizontallyValid = false;
        }

        var finalPoint = new Vector2Int(startPoint.X + depth, startPoint.Y + depth);
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

[Serializable]
public struct Vector2Int(int x, int y) : IEquatable<Vector2Int>
{
    public static Vector2Int One => new(1, 1);
    public static Vector2Int Left => new(-1, 0);
    public static Vector2Int Right => new(1, 0);
    public static Vector2Int Up => new(0, -1);
    public static Vector2Int Down => new(0, 1);
    public static Vector2Int Zero => new(0, 0);
    public static Vector2Int Max => new(int.MaxValue, int.MaxValue);

    public int X { get; internal set; } = x;
    public int Y { get; internal set; } = y;

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public bool Equals(Vector2Int other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2Int other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (X.GetHashCode() * 397) ^ Y.GetHashCode();
        }
    }

    public static Vector2Int operator +(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(
            Add(left.X, right.X),
            Add(left.Y, right.Y)
        );
    }

    public static Vector2Int operator -(Vector2Int left, Vector2Int right)
    {
        left.X -= right.X;
        left.Y -= right.Y;
        return left;
    }

    public static Vector2Int operator -(Vector2Int vec)
    {
        vec.X = -vec.X;
        vec.Y = -vec.Y;
        return vec;
    }

    public static Vector2Int operator *(Vector2Int vec, int scale) => vec * Convert.ToSingle(scale);

    public static Vector2Int operator *(int scale, Vector2Int vec) => vec * Convert.ToSingle(scale);

    public static Vector2Int operator *(Vector2Int vec, float scale) => scale * vec;

    public static Vector2Int operator *(float scale, Vector2Int vec)
    {
        vec.X = Convert.ToInt32(vec.X * scale);
        vec.Y = Convert.ToInt32(vec.Y * scale);
        return vec;
    }

    public static Vector2Int operator *(Vector2Int left, Vector2Int right)
    {
        left.X *= right.X;
        left.Y *= right.Y;
        return left;
    }

    public static bool operator ==(Vector2Int left, Vector2Int right) => left.Equals(right);

    public static bool operator !=(Vector2Int left, Vector2Int right) => !left.Equals(right);

    public static bool operator <(Vector2Int left, Vector2Int right)
    {
        return left.X == right.X
            ? left.Y < right.Y
            : left.X < right.X;
    }

    public static bool operator >(Vector2Int left, Vector2Int right)
    {
        return left.X == right.X
            ? left.Y > right.Y
            : left.X > right.X;
    }

    public static bool operator <=(Vector2Int left, Vector2Int right)
    {
        return left.X == right.X
            ? left.Y <= right.Y
            : left.X < right.X;
    }

    public static bool operator >=(Vector2Int left, Vector2Int right)
    {
        return left.X == right.X
            ? left.Y >= right.Y
            : left.X > right.X;
    }

    private static int Add(int a, int b)
    {
        return (a + b);
    }
}