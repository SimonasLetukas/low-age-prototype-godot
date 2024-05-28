using System;
using Godot;

/// <summary>
/// Used to pass around events globally when wiring them directly is not an option
/// </summary>
public class EventBus : Node
{
    #region Singleton

    public static EventBus Instance = null;
    
    public override void _Ready()
    {
        base._Ready();
        
        if (Instance is null)
        {
            Instance = this;
        }
    }

    #endregion Singleton

    #region Events

    public event Action<IPathfindingUpdatable, bool> PathfindingUpdated = delegate { };
    public void RaisePathfindingUpdated(IPathfindingUpdatable data, bool isAdded) => PathfindingUpdated(data, isAdded);

    #endregion Events
}