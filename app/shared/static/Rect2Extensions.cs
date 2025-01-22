using System.Collections.Generic;
using Godot;
using Area3D = LowAgeData.Domain.Common.Area;

public static class Rect2Extensions
{
    public static Rect2 ToGodotRect2(this Area3D area) 
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
}