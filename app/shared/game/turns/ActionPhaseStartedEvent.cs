using System;

public class ActionPhaseStartedEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required int Turn { get; init; }
}