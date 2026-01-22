using System;

public class AbilityExecutionCompletedEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required int PlayerId { get; init; }
    public required Guid AbilityExecutionRequestedEventId { get; init; }
}