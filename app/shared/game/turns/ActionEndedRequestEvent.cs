using System;

public class ActionEndedRequestEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid ActorInAction { get; init; }
}