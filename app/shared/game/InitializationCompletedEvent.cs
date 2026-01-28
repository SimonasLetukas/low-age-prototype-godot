using System;

public class InitializationCompletedEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required int RandomSeed { get; init; }
}