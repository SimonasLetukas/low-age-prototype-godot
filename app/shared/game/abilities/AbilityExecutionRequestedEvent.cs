using System;
using LowAgeData.Domain.Abilities;

public class AbilityExecutionRequestedEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid SourceActorId { get; init; }
    public required AbilityId AbilityId { get; init; }
    public required IAbilityFocus Focus { get; init; }
}