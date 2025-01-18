using System;
using Godot;
using low_age_data.Domain.Entities;

public partial class EntityPlacedEvent : IGameEvent
{
    public EntityPlacedEvent(EntityId blueprintId, Vector2 mapPosition, Guid instanceId, ActorRotation actorRotation)
    {
        BlueprintId = blueprintId;
        MapPosition = mapPosition;
        InstanceId = instanceId;
        ActorRotation = actorRotation;
    }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public EntityId BlueprintId { get; }
    public Vector2 MapPosition { get; }
    public Guid InstanceId { get; }
    public ActorRotation ActorRotation { get; }
}