using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Logic;

public sealed class TriggerHandler : IDisposable
{
    public event Action Triggered = delegate { };

    public static TriggerHandler Setup(Trigger trigger) => new(trigger);

    private readonly IList<Event> _events;
    private readonly ValidationHandler _validationHandler;

    private Player? _playerListener;
    private Effects? _effectHistoryListener;
    private EntityNode? _entityListener;

    private bool _disposed;

    private TriggerHandler(Trigger trigger)
    {
        _events = trigger.Events;
        _validationHandler = ValidationHandler.Validate(trigger.Validators);

        EventBus.Instance.EntityDestroyed += OnEntityDestroyed;
        EventBus.Instance.PlayerResourcesUpdated += OnPlayerResourcesUpdated;
    }
    
    public void Dispose()
    {
        if (_disposed)
            return;
        
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

    public TriggerHandler With(IList<Tiles.TileInstance?> tileSource)
    {
        _validationHandler.With(tileSource);
        return this;
    }

    public TriggerHandler With(Effects effectHistorySource)
    {
        _effectHistoryListener = effectHistorySource;
        return this;
    }

    public TriggerHandler With(EntityNode entitySource)
    {
        _entityListener = entitySource;
        return this;
    }
    
    private void OnEntityDestroyed(EntityNode entity)
    {
        return; // TODO just an example for now
        
        if ((_events.Any(e => e.Equals(Event.OriginIsDestroyed)) 
             && entity.Equals(_effectHistoryListener?.OriginEntityOrNull)) 
            || (_events.Any(e => e.Equals(Event.SourceIsDestroyed)) 
                && entity.Equals(_effectHistoryListener?.SourceEntityOrNull)))
        {
            // TODO check validators first
            // TODO how to handle AND requirement between events...?
            Triggered();
        }
    }
    
    private void OnPlayerResourcesUpdated(Player player, IList<Payment> currentStockpile)
    {
        if (player.Equals(_playerListener) is false)
            return;

        if (_validationHandler.Handle() is false)
            return;

        Triggered();
    }
}