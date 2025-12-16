using Godot;
using LowAgeData.Domain.Common;

/// <summary>
/// Master node for all map-related object management: instances, visuals and pathfinding of entities, tiles and
/// fx (e.g. particles, name pending).
/// </summary>
public partial class Map : Node2D
{
	[Export] public bool DebugEnabled { get; set; } = true;

	protected TurnPhase CurrentPhase { get; private set; } = null!;
	protected ActorNode? ActorInAction { get; private set; }
	
	public override void _Ready()
	{
		if (DebugEnabled) GD.Print($"{nameof(Map)}: entering");
		
		EventBus.Instance.PhaseStarted += OnPhaseStarted;
		EventBus.Instance.PhaseEnded += OnPhaseEnded;
		EventBus.Instance.ActionStarted += OnActionStarted;
		
		base._Ready();
	}

	public override void _ExitTree()
	{
		EventBus.Instance.PhaseStarted -= OnPhaseStarted;
		EventBus.Instance.PhaseEnded -= OnPhaseEnded;
		EventBus.Instance.ActionStarted -= OnActionStarted;
		
		base._ExitTree();
	}

	protected virtual void OnPhaseStarted(int turn, TurnPhase phase)
	{
		CurrentPhase = phase;
		if (CurrentPhase.Equals(TurnPhase.Planning))
			ActorInAction = null;
	}

	protected virtual void OnPhaseEnded(int turn, TurnPhase phase)
	{
		ActorInAction = null;
	}

	protected virtual void OnActionStarted(ActorNode actor)
	{
		ActorInAction = actor;
	}
}
