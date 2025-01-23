using System.Collections.Generic;
using Godot;

public static class Iterate
{
    /// <summary>
    /// Helper to return iterated <see cref="Vector2"/>s.
    /// </summary>
    /// <param name="to">Iterate until (exclusive)</param>
    public static IEnumerable<Vector2> Positions(Vector2 to) => Positions((int)to.x, (int)to.y);
    
    /// <summary>
    /// Helper to return iterated <see cref="Vector2"/>s.
    /// </summary>
    /// <param name="from">Iterate from (inclusive)</param>
    /// <param name="to">Iterate until (exclusive)</param>
    public static IEnumerable<Vector2> Positions(Vector2 from, Vector2 to) 
        => Positions((int)from.x, (int)from.y, (int)to.x, (int)to.y);
    
    /// <summary>
    /// Helper to return iterated <see cref="Vector2"/>s.
    /// </summary>
    /// <param name="toX">Iterate until X (exclusive)</param>
    /// <param name="toY">Iterate until Y (exclusive)</param>
    /// <returns></returns>
    public static IEnumerable<Vector2> Positions(int toX, int toY) => Positions(0, 0, toX, toY);
    
    /// <summary>
    /// Helper to return iterated <see cref="Vector2"/>s.
    /// </summary>
    /// <param name="fromX">Iterate from X (inclusive)</param>
    /// <param name="fromY">Iterate from Y (inclusive)</param>
    /// <param name="toX">Iterate until X (exclusive)</param>
    /// <param name="toY">Iterate until Y (exclusive)</param>
    /// <returns></returns>
    public static IEnumerable<Vector2> Positions(int fromX, int fromY, int toX, int toY)
    {
        for (var currentX = fromX; currentX < toX; currentX++)
        {
            for (var currentY = fromY; currentY < toY; currentY++)
            {
                yield return new Vector2(currentX, currentY);
            }
        }
    }

    public static IEnumerable<Vector2> AdjacentPositions(Vector2 center, bool excludeCenter = true)
        => AdjacentPositions((int)center.x, (int)center.y, excludeCenter);
    
    public static IEnumerable<Vector2> AdjacentPositions(int centerX, int centerY, bool excludeCenter = true)
    {
        for (var x = centerX - 1; x <= centerX + 1; x++)
        {
            for (var y = centerY - 1; y <= centerY + 1; y++)
            {
                if (excludeCenter && x == centerX && y == centerY)
                    continue;
                
                yield return new Vector2(x, y);
            }
        }
    }
}