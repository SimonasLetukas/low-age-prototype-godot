using Godot;

/// <summary>
/// Master node for all map-related communications: data, visuals, pathfinding.
/// </summary>
public class Map : Node2D
{
    [Signal] public delegate void StartingPositionsDeclared(Vector2[] startingPositions);
    
    public bool DebugEnabled { get; set; } = false;
    protected Pathfinding Pathfinding;

    public override void _Ready()
    {
        Pathfinding = GetNode<Pathfinding>(nameof(Pathfinding)); // TODO not tested if node can be found
    }
}
