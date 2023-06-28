using Godot;

public static class Vector2Extensions
{
    public static bool IsInBoundsOf(this Vector2 point, Vector2 bounds) 
        => !(point.x < 0) 
           && !(point.y < 0) 
           && !(point.x >= bounds.x) 
           && !(point.y >= bounds.y);
}