using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common.Shape;
using low_age_prototype_common;

public static class IShapeExtensions
{
    public static IEnumerable<Vector2<int>> ToPositions(this IShape shape, EntityNode entity, Vector2<int> mapSize)
    {
        return shape.ToPositions(
            entity.EntityPrimaryPosition, 
            mapSize, 
            entity);
    }
    
    public static IEnumerable<Vector2<int>> ToPositions(this IShape shape, Vector2<int> centerPoint, 
        Vector2<int> mapSize, EntityNode entity)
    {
        return shape.ToPositions(
            centerPoint, 
            mapSize, 
            entity.EntitySize,
            entity is ActorNode actor 
                ? actor.ActorRotation 
                : ActorRotation.BottomRight);
    }
    
    public static IEnumerable<Vector2<int>> ToPositions(this IShape shape, Vector2<int> centerPoint, 
        Vector2<int> mapSize, Vector2<int>? actorSize = null, ActorRotation? actorRotation = null)
    {
        List<Vector2<int>> positions;
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
            case LowAgeData.Domain.Common.Shape.Map _:
                return GetPositionsForMap(mapSize);
            default:
                throw new ArgumentOutOfRangeException(nameof(shape), shape, 
                    $"The type '{shape.GetType().Name}' of provided shape is not supported.");
        }

        positions.RemoveAll(p => p.X < 0 || p.Y < 0 || p.X >= mapSize.X || p.Y >= mapSize.Y);
        return positions;
    }

    private static List<Vector2<int>> GetPositionsForCircle(Circle circle, Vector2<int> centerPoint, 
        Vector2<int>? actorSize = null)
    {
        var includedPositions = new HashSet<Vector2<int>>();
        var excludedPositions = new HashSet<Vector2<int>>();
        var size = actorSize ?? new Vector2<int>(1, 1);
        var includedRadiusSquared = circle.Radius * circle.Radius;
        var excludedRadiusSquared = circle.IgnoreRadius * circle.IgnoreRadius;
        
        for (var sizeOffsetX = 0; sizeOffsetX < size.X; sizeOffsetX++)
        {
            for (var sizeOffsetY = 0; sizeOffsetY < size.Y; sizeOffsetY++)
            {
                var centerX = centerPoint.X + sizeOffsetX;
                var centerY = centerPoint.Y + sizeOffsetY;
                includedPositions = DrawCircle(circle.Radius, centerX, centerY, 
                    includedRadiusSquared, includedPositions);
                includedPositions.Add(new Vector2<int>(centerX, centerY));
                
                if (circle.IgnoreRadius < 0)
                    continue;

                excludedPositions = DrawCircle(circle.IgnoreRadius, centerX, centerY,
                    excludedRadiusSquared, excludedPositions);
                excludedPositions.Add(new Vector2<int>(centerX, centerY));
            }
        }
        
        includedPositions.ExceptWith(excludedPositions);
        return includedPositions.ToList();
    }

    private static HashSet<Vector2<int>> DrawCircle(int radius, int centerX, int centerY, int? radiusSquared = null, 
        HashSet<Vector2<int>> existingPositions = null)
    {
        radiusSquared = radiusSquared ?? radius * radius;
        var positions = existingPositions ?? new HashSet<Vector2<int>>();
        
        for (var x = -radius; x < radius + 1; x++)
        {
            // Bubbly circle:
            var hh = (int)Mathf.Sqrt(radiusSquared.Value - x * x + radius);
            // Sharp circle:
            //var hh = (int)Mathf.Sqrt(radiusSquared.Value - x * x);
            var rx = centerX + x;
            var ph = centerY + hh;

            for (var y = centerY - hh; y < ph + 1; y++)
                positions.Add(new Vector2<int>(rx, y));
        }

        return positions;
    }

    private static List<Vector2<int>> GetPositionsForCustom(Custom custom, Vector2<int> centerPoint, 
        ActorRotation? actorRotation = null)
    {
        // TODO implement rotation, be careful about non-symmetrical custom areas -- add unit tests
        var positions = new HashSet<Vector2<int>>();
        foreach (var area in custom.Areas)
        {
            for (var x = area.Start.X; x < area.Size.X + area.Start.X; x++)
            {
                for (var y = area.Start.Y; y < area.Size.Y + area.Start.Y; y++)
                {
                    positions.Add(new Vector2<int>(centerPoint.X + x, centerPoint.Y + y));
                }
            }
        }
        
        return positions.ToList();
    }
    
    private static List<Vector2<int>> GetPositionsForLine(Line line, Vector2<int> centerPoint, 
        Vector2<int>? actorSize = null, ActorRotation? actorRotation = null)
    {
        var positions = new List<Vector2<int>>();
        var size = actorSize ?? new Vector2<int>(1, 1);
        actorRotation = actorRotation ?? ActorRotation.BottomRight;
        var xAxis = actorRotation is ActorRotation.BottomRight 
                    || actorRotation is ActorRotation.TopLeft;
        var positiveGrowth = actorRotation is ActorRotation.BottomRight 
                             || actorRotation is ActorRotation.BottomLeft;
        var offset = positiveGrowth
            ? xAxis 
                ? size.X 
                : size.Y
            : 1;

        for (var length = offset; length < line.Length + offset; length++)
        {
            if (line.IgnoreLength >= 0 && length < line.IgnoreLength + offset)
                continue;

            for (var width = 0; width < (xAxis ? size.Y : size.X); width++)
            {
                var x = centerPoint.X + (xAxis 
                    ? length * (positiveGrowth ? 1 : -1)
                    : width);
                var y = centerPoint.Y + (xAxis 
                    ? width 
                    : length * (positiveGrowth ? 1 : -1));
                
                positions.Add(new Vector2<int>(x, y));
            }
        }

        if (line.IgnoreLength == -1 && line.Length > 0)
        {
            for (var x = 0; x < size.X; x++)
            {
                for (var y = 0; y < size.Y; y++)
                {
                    positions.Add(new Vector2<int>(centerPoint.X + x, centerPoint.Y + y));
                }
            }
        }
        
        return positions;
    }
    
    private static IEnumerable<Vector2<int>> GetPositionsForMap(Vector2<int> mapSize)
    {
        var positions = new List<Vector2<int>>();
        for (var x = 0; x < mapSize.X; x++)
        {
            for (var y = 0; y < mapSize.Y; y++)
            {
                positions.Add(new Vector2<int>(x, y));
            }
        }

        return positions;
    }
}