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

    public event Action<string> ValidationError = delegate { };
    public void RaiseValidationError(string message) => ValidationError(message);
    
    public event Action<int, TurnPhase> PhaseStarted = delegate { };
    public void RaisePhaseStarted(int turn, TurnPhase phase) => PhaseStarted(turn, phase);
    
    public event Action<int, TurnPhase> PhaseEnded = delegate { };
    public void RaisePhaseEnded(int turn, TurnPhase phase) => PhaseEnded(turn, phase);

    public event Action<ActorNode> ActionStarted = delegate { };
    public void RaiseActionStarted(ActorNode actor) => ActionStarted(actor);
    
    public event Action<ActorNode> ActionEnded = delegate { };
    public void RaiseActionEnded(ActorNode actor) => ActionEnded(actor);

    public event Action<IList<ActorNode>> InitiativeQueueUpdated = delegate { };
    public void RaiseInitiativeQueueUpdated(IList<ActorNode> actors) => InitiativeQueueUpdated(actors);

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

    public event Action<EntityNode> EntityDestroyed = delegate { };
    public void RaiseEntityDestroyed(EntityNode entity) => EntityDestroyed(entity);
    
    public event Action<IPathfindingUpdatable, bool> PathfindingUpdating = delegate { };
    public void RaisePathfindingUpdating(IPathfindingUpdatable data, bool isAdded) => PathfindingUpdating(data, isAdded);

    public event Action<Point> HighGroundPointCreated = delegate { };
    public void RaiseHighGroundPointCreated(Point point) 
        => HighGroundPointCreated(point);
    
    public event Action<Point> HighGroundPointRemoved = delegate { };
    public void RaiseHighGroundPointRemoved(Point point) => HighGroundPointRemoved(point);

    public event Action<Player, IList<Payment>> PlayerResourcesUpdated = delegate { };
    public void RaisePlayerResourcesUpdated(Player player, IList<Payment> currentStockpile) 
        => PlayerResourcesUpdated(player, currentStockpile);

    public event Action<Player, IList<Payment>, bool> PaymentRequested = delegate { };
    public void RaisePaymentRequested(Player player, IList<Payment> resourcesSpent, bool isRefund) 
        => PaymentRequested(player, resourcesSpent, isRefund);
    
    public event Action<IncomeProvider> IncomeProviderRegistered = delegate { };
    public void RaiseIncomeProviderRegistered(IncomeProvider provider) => IncomeProviderRegistered(provider);
    
    public event Action<IncomeProvider> IncomeProviderUnregistered = delegate { };
    public void RaiseIncomeProviderUnregistered(IncomeProvider provider) => IncomeProviderUnregistered(provider);
    
    public event Action<IncomeProvider, IList<Payment>> IncomeProviderPaymentUpdated = delegate { };
    public void RaiseIncomeProviderPaymentUpdated(IncomeProvider provider, IList<Payment> updatedPayment) => IncomeProviderPaymentUpdated(provider, updatedPayment);

    public event Action<bool> WhenFlattenedChanged = delegate { };
    public void RaiseWhenFlattenedChanged(bool to) => WhenFlattenedChanged(to);
    
    public event Action<bool> AfterFlattenedChanged = delegate { };
    public void RaiseAfterFlattenedChanged(bool to) => AfterFlattenedChanged(to);
    
    public event Action<EntityNode> MovementAttackOverlayChanged = delegate { };
    public void RaiseMovementAttackOverlayChanged(EntityNode selectedEntity) 
        => MovementAttackOverlayChanged(selectedEntity);

    #endregion Events
}