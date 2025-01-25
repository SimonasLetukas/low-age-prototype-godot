using System.Collections.Generic;

namespace LowAgeCommon
{
    public static class IterateVector2Int
    {
        /// <summary>
        /// Helper to return iterated <see cref="Vector2{T}"/>s. Starts from (0, 0).
        /// </summary>
        /// <param name="to">Iterate until (exclusive)</param>
        /// <param name="reversed">If true, the iteration is reversed to start from "<see cref="to"/>" (exclusive) and
        /// finish with "(0, 0)" (inclusive)</param>
        public static IEnumerable<Vector2<int>> Positions(Vector2<int> to, bool reversed = false)
            => Positions(to.X, to.Y);

        /// <summary>
        /// Helper to return iterated <see cref="Vector2{T}"/>s.
        /// </summary>
        /// <param name="from">Iterate from (inclusive)</param>
        /// <param name="to">Iterate until (exclusive)</param>
        /// <param name="reversed">If true, the iteration is reversed to start from "<see cref="to"/>" (exclusive) and
        /// finish with "<see cref="from"/>" (inclusive)</param>
        public static IEnumerable<Vector2<int>> Positions(Vector2<int> from, Vector2<int> to, bool reversed = false)
            => Positions(from.X, from.Y, to.X, to.Y, reversed);

        /// <summary>
        /// Helper to return iterated <see cref="Vector2{T}"/>s. Starts from (0, 0).
        /// </summary>
        /// <param name="toX">Iterate until X (exclusive)</param>
        /// <param name="toY">Iterate until Y (exclusive)</param>
        /// <param name="reversed">If true, the iteration is reversed to start from the "to" (exclusive) and finish
        /// with the "from" (inclusive)</param>
        /// <returns></returns>
        public static IEnumerable<Vector2<int>> Positions(int toX, int toY, bool reversed = false)
            => Positions(0, 0, toX, toY, reversed);

        /// <summary>
        /// Helper to return iterated <see cref="Vector2{T}"/>s.
        /// </summary>
        /// <param name="fromX">Iterate from X (inclusive)</param>
        /// <param name="fromY">Iterate from Y (inclusive)</param>
        /// <param name="toX">Iterate until X (exclusive)</param>
        /// <param name="toY">Iterate until Y (exclusive)</param>
        /// <param name="reversed">If true, the iteration is reversed to start from the "to" (exclusive) and finish
        /// with the "from" (inclusive)</param>
        /// <returns></returns>
        public static IEnumerable<Vector2<int>> Positions(int fromX, int fromY, int toX, int toY, bool reversed = false)
        {
            if (reversed)
            {
                for (var currentX = toX - 1; currentX >= fromX; currentX--)
                {
                    for (var currentY = toY - 1; currentY >= fromY; currentY--)
                    {
                        yield return new Vector2<int>(currentX, currentY);
                    }
                }
            }
            else
            {
                for (var currentX = fromX; currentX < toX; currentX++)
                {
                    for (var currentY = fromY; currentY < toY; currentY++)
                    {
                        yield return new Vector2<int>(currentX, currentY);
                    }
                }
            }
        }

        public static IEnumerable<Vector2<int>> AdjacentPositions(this Vector2<int> center, bool excludeCenter = true, 
            Vector2<int>? lowerBoundsToCrop = null, Vector2<int>? upperBoundsToCrop = null)
            => AdjacentPositions(center.X, center.Y, excludeCenter, lowerBoundsToCrop, upperBoundsToCrop);

        public static IEnumerable<Vector2<int>> AdjacentPositions(int centerX, int centerY, bool excludeCenter = true,
            Vector2<int>? lowerBoundsToCrop = null, Vector2<int>? upperBoundsToCrop = null)
        {
            for (var x = centerX - 1; x <= centerX + 1; x++)
            {
                for (var y = centerY - 1; y <= centerY + 1; y++)
                {
                    if (excludeCenter && x == centerX && y == centerY)
                        continue;
                    
                    if (lowerBoundsToCrop != null
                        && (x < lowerBoundsToCrop.Value.X || y < lowerBoundsToCrop.Value.Y))
                        continue;

                    if (upperBoundsToCrop != null
                        && (x >= upperBoundsToCrop.Value.X || y >= upperBoundsToCrop.Value.Y))
                        continue;
                    
                    yield return new Vector2<int>(x, y);
                }
            }
        }
    }
}