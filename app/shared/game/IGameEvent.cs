using System;

/// <summary>
/// Represents each player action in the game that has to be sent to server; or events needed for new game preparation.
/// </summary>
public interface IGameEvent
{
    Guid Id { get; init; } // init needed for JSON serialization
}