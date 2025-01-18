using System;
using System.Collections.Generic;
using Godot;

public partial class UnitMovedAlongPathEvent : IGameEvent
{
    public UnitMovedAlongPathEvent(Guid entityInstanceId, IEnumerable<Vector2> globalPath,
        IEnumerable<Vector2> path)
    {
        EntityInstanceId = entityInstanceId;
        GlobalPath = globalPath;
        Path3D = path;
    }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EntityInstanceId { get; }
    public IEnumerable<Vector2> GlobalPath { get; } // TODO perhaps this can be calculated in each client instead?
    public IEnumerable<Vector2> Path3D { get; }
}