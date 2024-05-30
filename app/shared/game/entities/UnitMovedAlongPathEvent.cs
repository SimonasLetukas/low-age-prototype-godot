using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class UnitMovedAlongPathEvent : IGameEvent
{
    public UnitMovedAlongPathEvent(Guid entityInstanceId, IEnumerable<Vector2> globalPath,
        IEnumerable<Point> path)
    {
        EntityInstanceId = entityInstanceId;
        GlobalPath = globalPath.ToArray();
        Path = path.ToArray();
    }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EntityInstanceId { get; }
    public IEnumerable<Vector2> GlobalPath { get; } // TODO perhaps this can be calculated in each client instead?
    public IEnumerable<Point> Path { get; }
}