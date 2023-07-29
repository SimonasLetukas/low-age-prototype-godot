using Godot;

public static class Rect2Extensions
{
    public static Rect2 ToGodotRect2(this low_age_data.Domain.Shared.Area area) 
        => new Rect2(area.Start.ToGodotVector2(), area.Size.ToGodotVector2());
}