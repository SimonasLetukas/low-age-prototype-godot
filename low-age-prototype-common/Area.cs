using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace low_age_prototype_common
{
    public static class AreaExtensions
    {
        public static Area TrimTo(this Area areaToTrim, Vector2<int> boundarySize)
            => new Area(
                Math.Max(0, areaToTrim.Start.X),
                Math.Max(0, areaToTrim.Start.Y),
                Math.Min(boundarySize.X, areaToTrim.Size.X),
                Math.Min(boundarySize.Y, areaToTrim.Size.Y));

        public static IList<Vector2<int>> ToList(this Area area)
        {
            var list = new List<Vector2<int>>();
            for (var x = area.Start.X; x < area.Start.X + area.Size.X; x++)
            for (var y = area.Start.Y; y < area.Start.Y + area.Size.Y; y++)
                list.Add(new Vector2<int>(x, y));
            return list;
        }

        public static IList<Area> RotateClockwiseInside(this IList<Area> areas, Vector2<int> bounds, int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                areas = RotateClockwiseInside(areas, bounds);
                bounds = new Vector2<int>(bounds.Y, bounds.X);
            }

            return areas;
        }

        public static IList<Area> RotateClockwiseInside(this IList<Area> areas, Vector2<int> bounds)
        {
            var positionMap = new Dictionary<Vector2<int>, Vector2<int>>();

            for (var x = 0; x < bounds.X; x++)
            {
                for (var y = 0; y < bounds.Y; y++)
                {
                    var currentPoint = new Vector2<int>(x, y);
                    var newX = bounds.Y - 1 - y;
                    var newY = x;
                    var rotatedPoint = new Vector2<int>(newX, newY);

                    positionMap[currentPoint] = rotatedPoint;
                }
            }

            var newAreas = new List<Area>(areas.Count);

            foreach (var area in areas)
            {
                if (positionMap.TryGetValue(area.Start, out var newPosition) is false)
                    continue;

                var newArea = new Area(
                    new Vector2<int>((newPosition.X - area.Size.Y) + 1, newPosition.Y),
                    new Vector2<int>(area.Size.Y, area.Size.X));

                newAreas.Add(newArea);
            }

            return newAreas;
        }
    }

    [Serializable]
    public struct Area
    {
        public Area(Vector2<int> size)
        {
            Start = new Vector2<int>(0, 0);
            Size = size;
        }

        [JsonConstructor]
        public Area(Vector2<int> start, Vector2<int> size)
        {
            Start = start;
            Size = size;
        }

        public Area(int startX, int startY, int sizeX, int sizeY)
        {
            Start = new Vector2<int>(startX, startY);
            Size = new Vector2<int>(sizeX, sizeY);
        }
        
        /// <summary>
        /// Starting coordinates. Starts from 0. Could also have negative values to indicate starting outside of
        /// the usual boundaries.
        /// </summary>
        public Vector2<int> Start { get; }

        /// <summary>
        /// Size of the area. Starts from 1.
        /// </summary>
        public Vector2<int> Size { get; }

        public override string ToString() => $"{Start}, {Size}";

        public IEnumerable<Vector2<int>> ToVectors() => IterateVector2Int.Positions(Start, Start + Size);
    }
}