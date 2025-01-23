using System.Collections.Generic;
using Godot;
using Godot.Collections;

public static class Colliders
{
    public static List<Area2D> GetAt(Vector2 globalPosition, World2D world)
    {
        var colliders = new List<Area2D>();
        
        var intersections = world.DirectSpaceState.IntersectPoint(globalPosition, 32, 
            new Array(), 0x7FFFFFFF, true, true);
        
        foreach (var intersection in intersections)
        {
            const string colliderKey = "collider";
            var dictionary = (Dictionary)intersection;
            if (dictionary is null || dictionary.Contains(colliderKey) is false)
                continue;

            var collider = (Area2D)dictionary[colliderKey];
            
            if (collider is null)
                continue;
            
            colliders.Add(collider);
        }

        return colliders;
    }
}