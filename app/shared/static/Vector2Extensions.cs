using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;

public static class Vector2Extensions
{
    public static bool IsInBoundsOf(this Vector2 point, Vector2 upperBounds)
        => point.IsInBoundsOf(Vector2.Zero, upperBounds);
    
    public static bool IsInBoundsOf(this Vector2 point, Vector2 lowerBounds, Vector2 upperBounds) 
        => point.X >= lowerBounds.X 
           && point.Y >= lowerBounds.Y 
           && point.X < upperBounds.X 
           && point.Y < upperBounds.Y;

    public static Rect2 Except(this Vector2 size, IEnumerable<Vector2> points)
    {
        var allPoints = new Rect2(Vector2.Zero, size).ToList();
        var remainingPoints = allPoints.Except(points).ToList();
        if (remainingPoints.IsEmpty())
            return new Rect2(0, 0, 0, 0);
        
        var smallestX = float.MaxValue;
        var smallestY = float.MaxValue;
        var highestX = float.MinValue;
        var highestY = float.MinValue;
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

        return new Rect2(smallestX, smallestY, highestX - smallestX + 1, highestY - smallestY + 1);
    }

    public static Vector2 ToGodotVector2<T>(this Vector2<T> domainVector2) where T : struct, IEquatable<T> 
        => new Vector2(Convert.ToSingle(domainVector2.X), Convert.ToSingle(domainVector2.Y));

    public static IList<Rect2> ToSquareRects(this IList<Vector2> points)
    {
        var rects = new List<Rect2>();
        var checkedPoints = points.ToDictionary(point => point, point => false);

        foreach (var point in points)
        {
            if (checkedPoints[point])
                continue;

            checkedPoints[point] = true;
            var startPoint = new Vector2(point.X, point.Y);
            var size = 1;
            while (NextExtensionExists(checkedPoints, startPoint, size))
                size++;
            
            rects.Add(new Rect2(startPoint, size, size));
        }

        return rects;
    }

    private static bool NextExtensionExists(IDictionary<Vector2, bool> points, Vector2 startPoint, int depth)
    {
        var validPoints = new List<Vector2>();
        var verticallyValid = true;
        for (var y = 0; y < depth; y++)
        {
            var point = new Vector2(startPoint.X + depth, startPoint.Y + y);
            if (points.ContainsKey(point) && points[point] is false)
                validPoints.Add(point);
            else
                verticallyValid = false;
        }
        var horizontallyValid = true;
        for (var x = 0; x < depth; x++)
        {
            var point = new Vector2(startPoint.X + x, startPoint.Y + depth);
            if (points.ContainsKey(point) && points[point] is false)
                validPoints.Add(point);
            else
                horizontallyValid = false;
        }

        var finalPoint = new Vector2(startPoint.X + depth, startPoint.Y + depth);
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