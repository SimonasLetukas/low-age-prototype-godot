using System;

public class ActionEndResolvedEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required int PlayerStableId { get; init; }
}