using System;
using System.Collections.Generic;
using Godot;

public class UnitMovedAlongPathEvent : IGameEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Vector2 CurrentEntityPosition { get; set; } // TODO use entity ID instead
    public ICollection<Vector2> GlobalPath { get; set; } // TODO perhaps this can be calculated in each client instead?
    public ICollection<Vector2> Path { get; set; }

    public UnitMovedAlongPathEvent(Vector2 currentEntityPosition, ICollection<Vector2> globalPath,
        ICollection<Vector2> path)
    {
        CurrentEntityPosition = currentEntityPosition;
        GlobalPath = globalPath;
        Path = path;
    }
}