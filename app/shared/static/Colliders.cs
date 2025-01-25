using System.Collections.Generic;
using Godot;
using Godot.Collections;

public static class Colliders
{
    public static List<Area2D> GetAt(Vector2 globalPosition, World2D world)
    {
        var colliders = new List<Area2D>();

        var query = new PhysicsPointQueryParameters2D
        {
            Position = globalPosition,
            CollisionMask = 0x7FFFFFFF,
            Exclude = [],
            CollideWithBodies = true,
            CollideWithAreas = true
        };

        var intersections = world.DirectSpaceState.IntersectPoint(query);

        foreach (var result in intersections)
        {
            if (result.TryGetValue("collider", out var collider) 
                && collider.As<GodotObject>() is Area2D area)
            {
                colliders.Add(area);
            }
        }

        return colliders;
    }
}