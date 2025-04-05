using System;

public class ActionEndedEvent : IGameEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public required Guid ActorInAction { get; init; }
}