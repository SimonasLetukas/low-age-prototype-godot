using System;
using Godot;
using low_age_data.Domain.Entities;

public class EntityPlacedEvent : IGameEvent
{
    public EntityPlacedEvent(EntityId blueprintId, Vector2 mapPosition, Guid instanceId)
    {
        BlueprintId = blueprintId;
        MapPosition = mapPosition;
        InstanceId = instanceId;
    }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public EntityId BlueprintId { get; set; }
    public Vector2 MapPosition { get; set; }
    public Guid InstanceId { get; set; }
}