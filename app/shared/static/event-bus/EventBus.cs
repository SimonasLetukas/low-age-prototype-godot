using System;
using System.Collections.Generic;
using Godot;
using LowAgeData.Domain.Common;
using LowAgeCommon;
using MultipurposePathfinding;

/// <summary>
/// Used to pass around events globally when wiring them directly is not an option
/// </summary>
public partial class EventBus : Node
{
    #region Singleton

    public static EventBus Instance = null!;
    
    public override void _Ready()
    {
        base._Ready();
        
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Instance is null)
        {
            Instance = this;
        }
    }

    #endregion Singleton

    #region Events
    
    public event Action<int, Phase> PhaseStarted = delegate { };
    public void RaisePhaseStarted(int turn, Phase phase) => PhaseStarted(turn, phase);
    
    public event Action<int, Phase> PhaseEnded = delegate { };
    public void RaisePhaseEnded(int turn, Phase phase) => PhaseEnded(turn, phase);

    public event Action<Vector2Int, Terrain, IList<EntityNode>?> NewTileFocused = delegate { };
    public void RaiseNewTileFocused(Vector2Int mapPosition, Terrain terrain, IList<EntityNode>? occupants)
        => NewTileFocused(mapPosition, terrain, occupants);
    
    public event Action<EntityNode> EntityPlaced = delegate { };
    public void RaiseEntityPlaced(EntityNode entity) => EntityPlaced(entity);
    
    public event Action<EntityNode, EntityNode, AttackType> EntityTargeted = delegate { };
    public void RaiseEntityTargeted(EntityNode target, EntityNode source, AttackType attackType) 
        => EntityTargeted(target, source, attackType);

    public event Action<EntityNode, int> EntityZIndexUpdated = delegate { };
    public void RaiseEntityZIndexUpdated(EntityNode entity, int zIndex) => EntityZIndexUpdated(entity, zIndex);

    public event Action<IPathfindingUpdatable, bool> PathfindingUpdating = delegate { };
    public void RaisePathfindingUpdating(IPathfindingUpdatable data, bool isAdded) => PathfindingUpdating(data, isAdded);

    public event Action<Point> HighGroundPointCreated = delegate { };
    public void RaiseHighGroundPointCreated(Point point) 
        => HighGroundPointCreated(point);
    
    public event Action<Point> HighGroundPointRemoved = delegate { };
    public void RaiseHighGroundPointRemoved(Point point) => HighGroundPointRemoved(point);

    public event Action<bool> WhenFlattenedChanged = delegate { };
    public void RaiseWhenFlattenedChanged(bool to) => WhenFlattenedChanged(to);
    
    public event Action<bool> AfterFlattenedChanged = delegate { };
    public void RaiseAfterFlattenedChanged(bool to) => AfterFlattenedChanged(to);
    
    public event Action<EntityNode> MovementAttackOverlayChanged = delegate { };
    public void RaiseMovementAttackOverlayChanged(EntityNode selectedEntity) 
        => MovementAttackOverlayChanged(selectedEntity);

    #endregion Events
}