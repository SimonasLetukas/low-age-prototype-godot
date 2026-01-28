using System;

public class EntityCandidatePlacementCancelledEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid InstanceId { get; init; }
    public required int PlayerId { get; init; }
}