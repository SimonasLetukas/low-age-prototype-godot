using System;

public partial class ClientFinishedInitializingEvent : IGameEvent
{
    public ClientFinishedInitializingEvent(int playerStableId)
    {
        PlayerStableId = playerStableId;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public int PlayerStableId { get; }
}
