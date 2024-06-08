using System;
using System.Collections.Generic;
using Godot;
using low_age_data.Domain.Common;

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

    public event Action<Vector2, Terrain, IList<EntityNode>> NewTileFocused = delegate { };
    public void RaiseNewTileFocused(Vector2 mapPosition, Terrain terrain, IList<EntityNode> occupants)
        => NewTileFocused(mapPosition, terrain, occupants);
    
    public event Action<EntityNode> EntityPlaced = delegate { };
    public void RaiseEntityPlaced(EntityNode entity) => EntityPlaced(entity);

    public event Action<IPathfindingUpdatable, bool> PathfindingUpdating = delegate { };
    public void RaisePathfindingUpdating(IPathfindingUpdatable data, bool isAdded) => PathfindingUpdating(data, isAdded);

    public event Action<Point> HighGroundPointCreated = delegate { };
    public void RaiseHighGroundPointCreated(Point point) => HighGroundPointCreated(point);
    
    public event Action<Point> HighGroundPointRemoved = delegate { };
    public void RaiseHighGroundPointRemoved(Point point) => HighGroundPointRemoved(point);

    public event Action<bool> WhenFlattenedChanged = delegate { };
    public void RaiseWhenFlattenedChanged(bool to) => WhenFlattenedChanged(to);
    
    public event Action<bool> AfterFlattenedChanged = delegate { };
    public void RaiseAfterFlattenedChanged(bool to) => AfterFlattenedChanged(to);

    #endregion Events
}