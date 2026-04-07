using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Logic;

public sealed class TriggerHandler : IDisposable
{
    public event Action Triggered = delegate { };

    public static TriggerHandler Setup(Trigger trigger) => new(trigger);

    private readonly Event _event;
    private readonly ValidationHandler _validationHandler;

    private Player? _playerListener;
    private Effects? _effectHistoryListener;
    private EntityNode? _entityListener;

    private bool _disposed;

    private TriggerHandler(Trigger trigger)
    {
        _event = trigger.Event;
        _validationHandler = ValidationHandler.Validate(trigger.Validators);

        EventBus.Instance.PhaseStarted += OnPhaseStarted;
        EventBus.Instance.EntityDestroyed += OnEntityDestroyed;
        EventBus.Instance.PlayerResourcesUpdated += OnPlayerResourcesUpdated;
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        
        EventBus.Instance.PhaseStarted -= OnPhaseStarted;
        EventBus.Instance.EntityDestroyed -= OnEntityDestroyed;
        EventBus.Instance.PlayerResourcesUpdated -= OnPlayerResourcesUpdated;
        
        _disposed = true;
    }

    public TriggerHandler With(Player playerSource)
    {
        _playerListener = playerSource;
        _validationHandler.With(playerSource);
        return this;
    }

    public TriggerHandler With(IEnumerable<ITargetable> targetSource)
    {
        _validationHandler.With(targetSource);
        return this;
    }

    public TriggerHandler With(Effects effectHistorySource)
    {
        _validationHandler.With(effectHistorySource);
        _effectHistoryListener = effectHistorySource;
        return this;
    }

    public TriggerHandler With(EntityNode entitySource)
    {
        _entityListener = entitySource;
        return this;
    }
    
    private void OnPhaseStarted(int turn, TurnPhase phase)
    {
        if (_event.Equals(Event.EntityStartedActionPhase) && phase.Equals(TurnPhase.Action))
            Triggered();

        if (_event.Equals(Event.EntityStartedPlanningPhase) && phase.Equals(TurnPhase.Planning))
            Triggered();
    }
    
    private void OnEntityDestroyed(EntityNode entity, EntityNode? source, bool triggersOnDeathBehaviours)
    {
        var gotOriginEntity = _event.Equals(Event.OriginIsDestroyed)
                              && entity.Equals(_effectHistoryListener?.OriginEntityOrNull);
        var gotSourceEntity = _event.Equals(Event.SourceIsDestroyed)
                              && entity.Equals(_effectHistoryListener?.SourceEntityOrNull);
        
        if ((gotOriginEntity || gotSourceEntity) is false)
            return;

        var validationResult = _validationHandler.Handle();
        if (validationResult.IsValid is false)
        {
            if (Log.DebugEnabled)
                Log.Info(nameof(TriggerHandler), nameof(OnEntityDestroyed), 
                    $"{_event} trigger failed validation for {entity} because '{validationResult.Message}'.");
            
            return;
        }

        Triggered();
    }
    
    private void OnPlayerResourcesUpdated(Player player, IList<Payment> currentStockpile)
    {
        if (_event.Equals(Event.PlayerResourcesStockpileUpdated) is false)
            return;
        
        if (player.Equals(_playerListener) is false)
            return;

        if (_validationHandler.Handle().IsValid is false)
            return;

        Triggered();
    }
}