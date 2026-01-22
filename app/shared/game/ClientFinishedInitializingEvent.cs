using System;

public partial class ClientFinishedInitializingEvent : IGameEvent
{
    public ClientFinishedInitializingEvent(int playerId)
    {
        PlayerId = playerId;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public int PlayerId { get; }
}
