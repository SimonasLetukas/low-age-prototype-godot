using System;
using System.Collections.Generic;
using LowAgeCommon;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities;

public class EntityPlacedResponseEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required EntityId BlueprintId { get; init; }
    public required Vector2Int MapPosition { get; init; }
    public required List<Payment> Cost { get; init; }
    public required Guid InstanceId { get; init; }
    public required ActorRotation ActorRotation { get; init; }
    public required int PlayerId { get; init; }
    public required int CreationToken { get; init; }

    public static EntityPlacedResponseEvent From(EntityPlacedRequestEvent request, int creationToken) => new()
    {
        BlueprintId = request.BlueprintId,
        MapPosition = request.MapPosition,
        Cost = request.Cost,
        InstanceId = request.InstanceId,
        ActorRotation = request.ActorRotation,
        PlayerId = request.PlayerId,
        CreationToken = creationToken
    };
}