using System;
using LowAgeData.Domain.Common;

public partial class EntityAttackedEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid SourceId { get; init; }
    public required Guid TargetId { get; init; }
    public required AttackType AttackType { get; init; }
}