using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Common;
using Area = low_age_data.Domain.Common.Area;

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
            rects = RotateClockwiseInside(rects, bounds);

        return rects;
    }

    public static IList<Rect2> RotateClockwiseInside(this IList<Rect2> rects, Vector2 bounds)
    {
        var newRects = new List<Rect2>();

        for (var x = 0; x < bounds.x; x++)
        {
            for (var y = 0; y < bounds.y; y++)
            {
                var currentPoint = new Vector2(x, y);
                var newX = (int)bounds.y - 1 - y;
                var newY = x;
                
                newRects.AddRange(rects
                    .Where(walkableArea => walkableArea.Position.IsEqualApprox(currentPoint))
                    .Select(walkableArea => new Rect2(
                        new Vector2((newX - walkableArea.Size.y) + 1, newY),
                        new Vector2(walkableArea.Size.y, walkableArea.Size.x))));
            }
        }

        return newRects;
    }
}