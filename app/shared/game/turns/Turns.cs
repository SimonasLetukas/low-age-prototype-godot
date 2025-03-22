using Godot;

/// <summary>
/// Responsible for keeping track of turns and phases.
///
/// This class is not a singleton to ensure constrained communication flow (Game -> Turns -> EventBus -> Listener).
/// </summary>
public partial class Turns : Node2D
{
	[Export] public bool DebugEnabled { get; set; } = false;
	
	private int Turn { get; set; }
	private Phase Phase { get; set; }

	public override void _Ready()
	{
		if (DebugEnabled) GD.Print($"{nameof(Turns)}: entering");
		
		base._Ready();
		
		
	}

	public override void _ExitTree()
	{
		
		
		base._ExitTree();
	}
	
	
}

public enum Phase
{
	Planning,
	Action,
}
