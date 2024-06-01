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

    public event Action<Point> HighGroundPointCreated = delegate { };
    public void RaiseHighGroundPointCreated(Point point) => HighGroundPointCreated(point);
    
    public event Action<Point> HighGroundPointRemoved = delegate { };
    public void RaiseHighGroundPointRemoved(Point point) => HighGroundPointRemoved(point);

    #endregion Events
}