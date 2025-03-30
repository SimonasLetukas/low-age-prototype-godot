using System;

public class InitializationCompletedEvent : IGameEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public required int RandomSeed { get; init; }
}