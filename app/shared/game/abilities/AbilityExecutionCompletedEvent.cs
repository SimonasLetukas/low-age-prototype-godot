using System;

public class AbilityExecutionCompletedEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required int PlayerStableId { get; init; }
    public required Guid AbilityExecutionRequestedEventId { get; init; }
}