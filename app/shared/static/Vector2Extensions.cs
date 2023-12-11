using System;
using Godot;

public static class Vector2Extensions
{
    public static bool IsInBoundsOf(this Vector2 point, Vector2 bounds) 
        => !(point.x < 0) 
           && !(point.y < 0) 
           && !(point.x >= bounds.x) 
           && !(point.y >= bounds.y);

    public static Vector2 ToGodotVector2<T>(this low_age_data.Domain.Shared.Vector2<T> domainVector2) where T : struct, IEquatable<T> 
        => new Vector2(Convert.ToSingle(domainVector2.X), Convert.ToSingle(domainVector2.Y));
}