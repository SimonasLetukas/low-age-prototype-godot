using Godot;

public static class Rect2Extensions
{
    public static Rect2 ToGodotRect2(this low_age_data.Domain.Shared.Area area) 
        => new Rect2(area.Start.ToGodotVector2(), area.Size.ToGodotVector2());

    public static Rect2 TrimTo(this Rect2 rectToTrim, Vector2 boundarySize) 
        => new Rect2(
            Mathf.Max(0, rectToTrim.Position.x), 
            Mathf.Max(0, rectToTrim.Position.y), 
            Mathf.Min(boundarySize.x, rectToTrim.Size.x), 
            Mathf.Min(boundarySize.y, rectToTrim.Size.y));
}