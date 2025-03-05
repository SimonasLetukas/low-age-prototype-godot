using System;
using LowAgeData.Domain.Entities;
using LowAgeCommon;

public class EntityPlacedEvent : IGameEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public required EntityId BlueprintId { get; init; }
    public required Vector2Int MapPosition { get; init; }
    public required Guid InstanceId { get; init; }
    public required ActorRotation ActorRotation { get; init; }
    public required int PlayerId { get; init; }
}