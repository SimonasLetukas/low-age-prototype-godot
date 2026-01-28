using System;
using System.Collections.Generic;

public class PlanningPhaseEndedResponseEvent : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required IList<Guid> CancelledCandidateEntities { get; init; }
}