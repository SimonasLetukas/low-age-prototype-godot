using System;
using System.Collections.Generic;
using Godot;

public class UnitMovedAlongPathEvent : IGameEvent
{
    public UnitMovedAlongPathEvent(Guid entityInstanceId, ICollection<Vector2> globalPath,
        ICollection<Vector2> path)
    {
        EntityInstanceId = entityInstanceId;
        GlobalPath = globalPath;
        Path = path;
    }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EntityInstanceId { get; set; }
    public ICollection<Vector2> GlobalPath { get; set; } // TODO perhaps this can be calculated in each client instead?
    public ICollection<Vector2> Path { get; set; }
}