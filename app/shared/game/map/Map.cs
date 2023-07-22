using Godot;

/// <summary>
/// Master node for all map-related communications: data, visuals, pathfinding.
/// </summary>
public class Map : Node2D
{
    [Export] public bool DebugEnabled { get; set; } = true;
    
    [Signal] public delegate void StartingPositionsDeclared(Vector2[] startingPositions);
    
    protected Pathfinding Pathfinding;

    public override void _Ready()
    {
        if (DebugEnabled) GD.Print($"{nameof(Map)}: entering");
        Pathfinding = GetNode<Pathfinding>(nameof(Pathfinding));
        if (DebugEnabled) GD.Print($"{nameof(Map)}: found node {nameof(Pathfinding)} '{Pathfinding}'");
    }
}
