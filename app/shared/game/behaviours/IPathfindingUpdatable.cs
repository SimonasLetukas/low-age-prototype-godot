using System.Collections.Generic;
using Godot;

public interface IPathfindingUpdatable
{
    /// <summary>
    /// Returns a list of leveled nodes to update the pathfinding: inner List&lt;Vector2&gt; describes one level
    /// of node positions, while the outer List describes the levels (first entry is the highest level, last entry
    /// is the lowest) and their Y sprite offset. 
    /// </summary>
    /// <returns></returns>
    List<(IList<Vector2>, int)> LeveledPositions { get; }

    bool CanBeMovedOnAt(Vector2 position);
}