using System.Collections.Generic;
using Godot;

public interface IPathfindingUpdatable : IBehaviour
{
    /// <summary>
    /// Returns a list of leveled nodes to update the pathfinding: inner IEnumerable&lt;Vector2&gt; describes one level
    /// of node positions, while the outer IEnumerable describes the levels (first entry is the highest level, last entry
    /// is the lowest) and their Y sprite offset. 
    /// </summary>
    /// <returns></returns>
    IList<(IEnumerable<Vector2>, int)> LeveledPositions { get; }
    
    /// <summary>
    /// Same as <see cref="LeveledPositions"/>, but positions are local (start at (0, 0)).
    /// </summary>
    IList<(IEnumerable<Vector2>, int)> LeveledLocalPositions { get; }
    
    /// <summary>
    /// Returns a non-leveled (flattened) set of positions and their Y sprite offset.
    /// </summary>
    Dictionary<Vector2, int> FlattenedPositions { get; }
    
    /// <summary>
    /// Same as <see cref="FlattenedPositions"/>, but positions are local (start at (0, 0)).
    /// </summary>
    Dictionary<Vector2, int> FlattenedLocalPositions { get; }

    bool CanBeMovedOnAt(Vector2 position);
}