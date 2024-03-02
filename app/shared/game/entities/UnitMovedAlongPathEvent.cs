using System;
using System.Collections.Generic;
using Godot;

public class UnitMovedAlongPathEvent : IGameEvent
{
    public UnitMovedAlongPathEvent(Guid entityInstanceId, IEnumerable<Vector2> globalPath,
        IEnumerable<Vector2> path)
    {
        EntityInstanceId = entityInstanceId;
        GlobalPath = globalPath;
        Path = path;
    }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EntityInstanceId { get; set; }
    public IEnumerable<Vector2> GlobalPath { get; set; } // TODO perhaps this can be calculated in each client instead?
    public IEnumerable<Vector2> Path { get; set; }
}