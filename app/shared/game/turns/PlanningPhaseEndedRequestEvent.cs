using System;
using System.Collections.Generic;
using LowAgeCommon;

public class PlanningPhaseEndedRequestEvent : IGameEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public required int Turn { get; init; }
    public required int PlayerId { get; init; }
    public required IList<CandidateEntity> CandidateEntities { get; init; }
}

public class CandidateEntity
{
    public required Guid EntityId { get; init; }
    public required IList<Vector2Int> OccupyingPositions { get; init; }
}