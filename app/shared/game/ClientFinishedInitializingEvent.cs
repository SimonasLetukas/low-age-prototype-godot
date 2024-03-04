using System;

public class ClientFinishedInitializingEvent : IGameEvent
{
    public ClientFinishedInitializingEvent(int playerId)
    {
        PlayerId = playerId;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public int PlayerId { get; }
}
