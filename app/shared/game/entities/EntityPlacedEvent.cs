using System;
using LowAgeData.Domain.Entities;
using LowAgeCommon;

public partial class EntityPlacedEvent : IGameEvent
{
    public EntityPlacedEvent(EntityId blueprintId, Vector2Int mapPosition, Guid instanceId, 
        ActorRotation actorRotation)
    {
        BlueprintId = blueprintId;
        MapPosition = mapPosition;
        InstanceId = instanceId;
        ActorRotation = actorRotation;
    }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public EntityId BlueprintId { get; }
    public Vector2Int MapPosition { get; }
    public Guid InstanceId { get; }
    public ActorRotation ActorRotation { get; }
}