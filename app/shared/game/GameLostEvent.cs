using System;

public class GameLostEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required int PlayerStableId { get; init; }
}