using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Common.Shape;

public static class IShapeExtensions
{
    public static IEnumerable<Vector2> ToPositions(this IShape shape, EntityNode entity, Vector2 mapSize)
    {
        return shape.ToPositions(
            entity.EntityPrimaryPosition, 
            mapSize, 
            entity);
    }
    
    public static IEnumerable<Vector2> ToPositions(this IShape shape, Vector2 centerPoint, Vector2 mapSize, EntityNode entity)
    {
        return shape.ToPositions(
            centerPoint, 
            mapSize, 
            entity.EntitySize,
            entity is ActorNode actor 
                ? actor.ActorRotation 
                : ActorRotation.BottomRight);
    }
    
    public static IEnumerable<Vector2> ToPositions(this IShape shape, Vector2 centerPoint, Vector2 mapSize, Vector2? actorSize = null, ActorRotation? actorRotation = null)
    {
        List<Vector2> positions;
        switch (shape)
        {
            case Circle circle:
                positions = GetPositionsForCircle(circle, centerPoint, actorSize);
                break;
            case Custom custom:
                positions = GetPositionsForCustom(custom, centerPoint, actorRotation);
                break;
            case Line line:
                positions = GetPositionsForLine(line, centerPoint, actorSize, actorRotation);
                break;
            case low_age_data.Domain.Common.Shape.Map _:
                return GetPositionsForMap(mapSize);
            default:
                throw new ArgumentOutOfRangeException(nameof(shape), shape, 
                    $"The type '{shape.GetType().Name}' of provided shape is not supported.");
        }

        positions.RemoveAll(p => p.x < 0 || p.y < 0 || p.x >= mapSize.x || p.y >= mapSize.y);
        return positions;
    }

    private static List<Vector2> GetPositionsForCircle(Circle circle, Vector2 centerPoint, Vector2? actorSize = null)
    {
        var includedPositions = new HashSet<Vector2>();
        var excludedPositions = new HashSet<Vector2>();
        var size = actorSize ?? new Vector2(1, 1);
        var includedRadiusSquared = circle.Radius * circle.Radius;
        var excludedRadiusSquared = circle.IgnoreRadius * circle.IgnoreRadius;
        
        for (var sizeOffsetX = 0; sizeOffsetX < size.x; sizeOffsetX++)
        {
            for (var sizeOffsetY = 0; sizeOffsetY < size.y; sizeOffsetY++)
            {
                var centerX = centerPoint.x + sizeOffsetX;
                var centerY = centerPoint.y + sizeOffsetY;
                includedPositions = DrawCircle(circle.Radius, (int)centerX, (int)centerY, 
                    includedRadiusSquared, includedPositions);
                includedPositions.Add(new Vector2(centerX, centerY));
                
                if (circle.IgnoreRadius < 0)
                    continue;

                excludedPositions = DrawCircle(circle.IgnoreRadius, (int)centerX, (int)centerY,
                    excludedRadiusSquared, excludedPositions);
                excludedPositions.Add(new Vector2(centerX, centerY));
            }
        }
        
        includedPositions.ExceptWith(excludedPositions);
        return includedPositions.ToList();
    }

    private static HashSet<Vector2> DrawCircle(int radius, int centerX, int centerY, int? radiusSquared = null, 
        HashSet<Vector2> existingPositions = null)
    {
        radiusSquared = radiusSquared ?? radius * radius;
        var positions = existingPositions ?? new HashSet<Vector2>();
        
        for (var x = -radius; x < radius + 1; x++)
        {
            // Bubbly circle:
            var hh = (int)Mathf.Sqrt(radiusSquared.Value - x * x + radius);
            // Sharp circle:
            //var hh = (int)Mathf.Sqrt(radiusSquared.Value - x * x);
            var rx = centerX + x;
            var ph = centerY + hh;

            for (var y = centerY - hh; y < ph + 1; y++)
                positions.Add(new Vector2(rx, y));
        }

        return positions;
    }

    private static List<Vector2> GetPositionsForCustom(Custom custom, Vector2 centerPoint, ActorRotation? actorRotation = null)
    {
        // TODO implement rotation, be careful about non-symmetrical custom areas -- add unit tests
        var positions = new HashSet<Vector2>();
        foreach (var area in custom.Areas)
        {
            for (var x = area.Start.X; x < area.Size.X + area.Start.X; x++)
            {
                for (var y = area.Start.Y; y < area.Size.Y + area.Start.Y; y++)
                {
                    positions.Add(new Vector2(centerPoint.x + x, centerPoint.y + y));
                }
            }
        }
        
        return positions.ToList();
    }
    
    private static List<Vector2> GetPositionsForLine(Line line, Vector2 centerPoint, Vector2? actorSize = null, ActorRotation? actorRotation = null)
    {
        var positions = new List<Vector2>();
        var size = actorSize ?? new Vector2(1, 1);
        actorRotation = actorRotation ?? ActorRotation.BottomRight;
        var xAxis = actorRotation is ActorRotation.BottomRight 
                    || actorRotation is ActorRotation.TopLeft;
        var positiveGrowth = actorRotation is ActorRotation.BottomRight 
                             || actorRotation is ActorRotation.BottomLeft;
        var offset = positiveGrowth
            ? xAxis 
                ? size.x 
                : size.y
            : 1;

        for (var length = (int)offset; length < line.Length + offset; length++)
        {
            if (line.IgnoreLength >= 0 && length < line.IgnoreLength + offset)
                continue;

            for (var width = 0; width < (xAxis ? size.y : size.x); width++)
            {
                var x = centerPoint.x + (xAxis 
                    ? length * (positiveGrowth ? 1 : -1)
                    : width);
                var y = centerPoint.y + (xAxis 
                    ? width 
                    : length * (positiveGrowth ? 1 : -1));
                
                positions.Add(new Vector2(x, y));
            }
        }

        if (line.IgnoreLength == -1 && line.Length > 0)
        {
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    positions.Add(new Vector2(centerPoint.x + x, centerPoint.y + y));
                }
            }
        }
        
        return positions;
    }
    
    private static IEnumerable<Vector2> GetPositionsForMap(Vector2 mapSize)
    {
        var positions = new List<Vector2>();
        for (var x = 0; x < mapSize.x; x++)
        {
            for (var y = 0; y < mapSize.y; y++)
            {
                positions.Add(new Vector2(x, y));
            }
        }

        return positions;
    }
}