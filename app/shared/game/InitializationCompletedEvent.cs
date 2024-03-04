using System;

public class InitializationCompletedEvent : IGameEvent
{
    public InitializationCompletedEvent()
    {
    }
    
    public Guid Id { get; set; } = Guid.NewGuid();
}