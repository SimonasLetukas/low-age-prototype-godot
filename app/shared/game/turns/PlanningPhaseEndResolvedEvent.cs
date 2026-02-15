using System;

public class PlanningPhaseEndResolvedEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required int PlayerStableId { get; init; }
}