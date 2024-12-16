using System.Collections.Generic;

namespace low_age_prototype_common
{
    public static class IterateVector2Int
    {
        /// <summary>
        /// Helper to return iterated <see cref="Vector2{T}"/>s.
        /// </summary>
        /// <param name="to">Iterate until (exclusive)</param>
        public static IEnumerable<Vector2<int>> Positions(Vector2<int> to)
            => Positions(to.X, to.Y);

        /// <summary>
        /// Helper to return iterated <see cref="Vector2{T}"/>s.
        /// </summary>
        /// <param name="from">Iterate from (inclusive)</param>
        /// <param name="to">Iterate until (exclusive)</param>
        public static IEnumerable<Vector2<int>> Positions(Vector2<int> from, Vector2<int> to)
            => Positions(from.X, from.Y, to.X, to.Y);

        /// <summary>
        /// Helper to return iterated <see cref="Vector2{T}"/>s.
        /// </summary>
        /// <param name="toX">Iterate until X (exclusive)</param>
        /// <param name="toY">Iterate until Y (exclusive)</param>
        /// <returns></returns>
        public static IEnumerable<Vector2<int>> Positions(int toX, int toY)
            => Positions(0, 0, toX, toY);

        /// <summary>
        /// Helper to return iterated <see cref="Vector2{T}"/>s.
        /// </summary>
        /// <param name="fromX">Iterate from X (inclusive)</param>
        /// <param name="fromY">Iterate from Y (inclusive)</param>
        /// <param name="toX">Iterate until X (exclusive)</param>
        /// <param name="toY">Iterate until Y (exclusive)</param>
        /// <returns></returns>
        public static IEnumerable<Vector2<int>> Positions(int fromX, int fromY, int toX, int toY)
        {
            for (var currentX = fromX; currentX < toX; currentX++)
            {
                for (var currentY = fromY; currentY < toY; currentY++)
                {
                    yield return new Vector2<int>(currentX, currentY);
                }
            }
        }

        public static IEnumerable<Vector2<int>> AdjacentPositions(Vector2<int> center, bool excludeCenter = true)
            => AdjacentPositions(center.X, center.Y, excludeCenter);

        public static IEnumerable<Vector2<int>> AdjacentPositions(int centerX, int centerY, bool excludeCenter = true)
        {
            for (var x = centerX - 1; x <= centerX + 1; x++)
            {
                for (var y = centerY - 1; y <= centerY + 1; y++)
                {
                    if (excludeCenter && x == centerX && y == centerY)
                        continue;

                    yield return new Vector2<int>(x, y);
                }
            }
        }
    }
}