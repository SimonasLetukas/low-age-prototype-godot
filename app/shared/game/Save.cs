using System;
using System.Collections.Generic;
using LowAgeData.Domain.Factions;

public record Save
{
    public required Guid GameId { get; init; }
    public required string MapLocation { get; init; }
    public required DateTimeOffset SavedAtUtc { get; init; }
    public required Dictionary<int, SavePlayer> Players { get; init; }
    public required IList<string> Events { get; init; }
}

public record SavePlayer
{
    public required FactionId FactionId { get; init; }
    public required int Team { get; init; }
}