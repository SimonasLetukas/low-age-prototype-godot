using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LowAgeCommon
{
    public static class AreaExtensions
    {
        public static Area TrimTo(this Area areaToTrim, Vector2Int boundarySize)
            => new Area(
                Math.Max(0, areaToTrim.Start.X),
                Math.Max(0, areaToTrim.Start.Y),
                Math.Min(boundarySize.X, areaToTrim.Size.X),
                Math.Min(boundarySize.Y, areaToTrim.Size.Y));

        public static IList<Vector2Int> ToList(this Area area)
        {
            var list = new List<Vector2Int>();
            for (var x = area.Start.X; x < area.Start.X + area.Size.X; x++)
            for (var y = area.Start.Y; y < area.Start.Y + area.Size.Y; y++)
                list.Add(new Vector2Int(x, y));
            return list;
        }

        public static IList<Area> RotateClockwiseInside(this IList<Area> areas, Vector2Int bounds, int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                areas = RotateClockwiseInside(areas, bounds);
                bounds = new Vector2Int(bounds.Y, bounds.X);
            }

            return areas;
        }

        public static IList<Area> RotateClockwiseInside(this IList<Area> areas, Vector2Int bounds)
        {
            var positionMap = new Dictionary<Vector2Int, Vector2Int>();

            for (var x = 0; x < bounds.X; x++)
            {
                for (var y = 0; y < bounds.Y; y++)
                {
                    var currentPoint = new Vector2Int(x, y);
                    var newX = bounds.Y - 1 - y;
                    var newY = x;
                    var rotatedPoint = new Vector2Int(newX, newY);

                    positionMap[currentPoint] = rotatedPoint;
                }
            }

            var newAreas = new List<Area>(areas.Count);

            foreach (var area in areas)
            {
                if (positionMap.TryGetValue(area.Start, out var newPosition) is false)
                    continue;

                var newArea = new Area(
                    new Vector2Int((newPosition.X - area.Size.Y) + 1, newPosition.Y),
                    new Vector2Int(area.Size.Y, area.Size.X));

                newAreas.Add(newArea);
            }

            return newAreas;
        }
    }

    [Serializable]
    public struct Area
    {
        public Area(Vector2Int size)
        {
            Start = new Vector2Int(0, 0);
            Size = size;
        }

        [JsonConstructor]
        public Area(Vector2Int start, Vector2Int size)
        {
            Start = start;
            Size = size;
        }

        public Area(int startX, int startY, int sizeX, int sizeY)
        {
            Start = new Vector2Int(startX, startY);
            Size = new Vector2Int(sizeX, sizeY);
        }
        
        /// <summary>
        /// Starting coordinates. Starts from 0. Could also have negative values to indicate starting outside of
        /// the usual boundaries.
        /// </summary>
        public Vector2Int Start { get; }

        /// <summary>
        /// Size of the area. Starts from 1.
        /// </summary>
        public Vector2Int Size { get; }

        public override string ToString() => $"{Start}, {Size}";

        public IEnumerable<Vector2Int> ToVectors() => IterateVector2Int.Positions(Start, Start + Size);
    }
}