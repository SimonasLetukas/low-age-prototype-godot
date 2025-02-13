using System.Collections.Generic;
using LowAgeCommon;
using MultipurposePathfinding;

public interface IPathfindingUpdatable : IBehaviour
{
    /// <summary>
    /// Returns a list of leveled nodes to update the pathfinding: inner IEnumerable&lt;Vector2&gt; describes one level
    /// of node positions, while the outer IEnumerable describes the levels (first entry is the highest level, last entry
    /// is the lowest) and their Y sprite offset. 
    /// </summary>
    IList<(IEnumerable<Vector2Int>, int)> LeveledPositions { get; }
    
    /// <summary>
    /// Returns a list of leveled nodes to update the pathfinding: inner IEnumerable&lt;Vector2&gt; describes one level
    /// of node positions, while the outer IEnumerable describes the levels (first entry is the highest level, last entry
    /// is the lowest). 
    /// </summary>
    IList<IEnumerable<Vector2Int>> LeveledPositionsWithoutSpriteOffset { get; }
    
    /// <summary>
    /// Same as <see cref="LeveledPositions"/>, but positions are local (start at (0, 0)).
    /// </summary>
    IList<(IEnumerable<Vector2Int>, int)> LeveledLocalPositions { get; }
    
    /// <summary>
    /// Returns a non-leveled (flattened) set of positions and their Y sprite offset.
    /// </summary>
    Dictionary<Vector2Int, int> FlattenedPositions { get; }
    
    /// <summary>
    /// Returns a non-leveled (flattened) set of positions.
    /// </summary>
    IEnumerable<Vector2Int> FlattenedPositionsWithoutSpriteOffset { get; }
    
    /// <summary>
    /// Same as <see cref="FlattenedPositions"/>, but positions are local (start at (0, 0)).
    /// </summary>
    Dictionary<Vector2Int, int> FlattenedLocalPositions { get; }

    bool CanBeMovedOnAt(Vector2Int position, Team forTeam);

    bool AllowsConnectionBetweenPoints(Point fromPoint, Point toPoint, Team forTeam);
}