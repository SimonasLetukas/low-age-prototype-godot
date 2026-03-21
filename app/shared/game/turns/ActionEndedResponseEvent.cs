using System;

public class ActionEndedResponseEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid ActorInAction { get; init; }
}