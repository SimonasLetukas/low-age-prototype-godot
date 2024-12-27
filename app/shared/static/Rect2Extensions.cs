using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Common;
using Newtonsoft.Json;
using Area = low_age_prototype_common.Area;

public static class Rect2Extensions
{
    public static Rect2 ToGodotRect2(this Area area) 
        => new Rect2(area.Start.ToGodotVector2(), area.Size.ToGodotVector2());

    public static Rect2 TrimTo(this Rect2 rectToTrim, Vector2 boundarySize) 
        => new Rect2(
            Mathf.Max(0, rectToTrim.Position.x), 
            Mathf.Max(0, rectToTrim.Position.y), 
            Mathf.Min(boundarySize.x, rectToTrim.Size.x), 
            Mathf.Min(boundarySize.y, rectToTrim.Size.y));

    public static IList<Vector2> ToList(this Rect2 rect)
    {
        var list = new List<Vector2>();
        for (var x = (int)rect.Position.x; x < (int)(rect.Position.x + rect.Size.x); x++)
            for (var y = (int)rect.Position.y; y < (int)(rect.Position.y + rect.Size.y); y++) 
                list.Add(new Vector2(x, y));
        return list;
    }

    public static IList<Rect2> RotateClockwiseInside(this IList<Rect2> rects, Vector2 bounds, int iterations)
    {
        for (var i = 0; i < iterations; i++)
        {
            rects = RotateClockwiseInside(rects, bounds);
            bounds = new Vector2(bounds.y, bounds.x);
        }

        return rects;
    }
    
    public static IList<Rect2> RotateClockwiseInside(this IList<Rect2> rects, Vector2 bounds)
    {
        var positionMap = new Dictionary<Vector2, Vector2>();

        for (var x = 0; x < bounds.x; x++)
        {
            for (var y = 0; y < bounds.y; y++)
            {
                var currentPoint = new Vector2(x, y);
                var newX = (int)bounds.y - 1 - y;
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
                new Vector2((newPosition.x - rect.Size.y) + 1, newPosition.y),
                new Vector2(rect.Size.y, rect.Size.x));
            
            newRects.Add(newRect);
        }

        return newRects;
    }
}