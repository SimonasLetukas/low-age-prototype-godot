using System.Collections.Generic;
using Godot;
using Area = LowAgeCommon.Area;

public static class Rect2Extensions
{
    public static Rect2 ToGodotRect2(this Area area) 
        => new Rect2(area.Start.ToGodotVector2(), area.Size.ToGodotVector2());

    public static Rect2 TrimTo(this Rect2 rectToTrim, Vector2 boundarySize) 
        => new Rect2(
            Mathf.Max(0, rectToTrim.Position.X), 
            Mathf.Max(0, rectToTrim.Position.Y), 
            Mathf.Min(boundarySize.X, rectToTrim.Size.X), 
            Mathf.Min(boundarySize.Y, rectToTrim.Size.Y));

    public static IList<Vector2> ToList(this Rect2 rect)
    {
        var list = new List<Vector2>();
        for (var x = (int)rect.Position.X; x < (int)(rect.Position.X + rect.Size.X); x++)
            for (var y = (int)rect.Position.Y; y < (int)(rect.Position.Y + rect.Size.Y); y++) 
                list.Add(new Vector2(x, y));
        return list;
    }

    public static IList<Rect2> RotateClockwiseInside(this IList<Rect2> rects, Vector2 bounds, int iterations)
    {
        for (var i = 0; i < iterations; i++)
        {
            rects = RotateClockwiseInside(rects, bounds);
            bounds = new Vector2(bounds.Y, bounds.X);
        }

        return rects;
    }
    
    public static IList<Rect2> RotateClockwiseInside(this IList<Rect2> rects, Vector2 bounds)
    {
        var positionMap = new Dictionary<Vector2, Vector2>();

        for (var x = 0; x < bounds.X; x++)
        {
            for (var y = 0; y < bounds.Y; y++)
            {
                var currentPoint = new Vector2(x, y);
                var newX = (int)bounds.Y - 1 - y;
                var newY = x;
                var rotatedPoint = new Vector2(newX, newY);

                positionMap[currentPoint] = rotatedPoint;
            }
        }

        var newRects = new List<Rect2>(rects.Count);

        foreach (var rect in rects)
        {
            if (positionMap.TryGetValue(rect.Position, out var newPosition) is false) 
                continue;
            
            var newRect = new Rect2(
                new Vector2((newPosition.X - rect.Size.Y) + 1, newPosition.Y),
                new Vector2(rect.Size.Y, rect.Size.X));
            
            newRects.Add(newRect);
        }

        return newRects;
    }
}