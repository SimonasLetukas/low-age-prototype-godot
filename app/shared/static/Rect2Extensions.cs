using System.Collections.Generic;
using Godot;
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
}