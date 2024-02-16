using System;

public class EntityPlacedEvent : IGameEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
}