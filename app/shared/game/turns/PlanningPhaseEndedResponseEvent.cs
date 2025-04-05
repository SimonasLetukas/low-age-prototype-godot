using System;
using System.Collections.Generic;

public class PlanningPhaseEndedResponseEvent : IGameEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public required int Turn { get; init; }
    public required IList<Guid> CancelledCandidateEntities { get; init; }
}