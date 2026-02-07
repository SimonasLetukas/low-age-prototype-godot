using System;
using System.Collections.Generic;
using LowAgeData.Domain.Entities;
using LowAgeCommon;
using LowAgeData.Domain.Common;

public class EntityPlacedRequestEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required EntityId BlueprintId { get; init; }
    public required Vector2Int MapPosition { get; init; }
    public required List<Payment> Cost { get; init; }
    public required Guid InstanceId { get; init; }
    public required ActorRotation ActorRotation { get; init; }
    public required int PlayerId { get; init; }
}