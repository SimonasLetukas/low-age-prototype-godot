using Godot;

/// <summary>
/// Master node for all map-related object management: instances, visuals and pathfinding of entities, tiles and
/// fx (e.g. particles, name pending).
/// </summary>
public partial class Map : Node2D
{
	[Export] public bool DebugEnabled { get; set; } = true;
	
	public override void _Ready()
	{
		if (DebugEnabled) GD.Print($"{nameof(Map)}: entering");
		
		base._Ready();
	}
}
