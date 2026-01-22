using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MultipurposePathfinding;

public partial class UnitMovedAlongPathEvent(
    Guid entityInstanceId,
    IEnumerable<Vector2> globalPath,
    IEnumerable<Point> path)
    : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid EntityInstanceId { get; } = entityInstanceId;
    public IEnumerable<Vector2> GlobalPath { get; } = globalPath.ToArray(); // TODO perhaps this can be calculated in each client instead?
    public IEnumerable<Point> Path { get; } = path.ToArray();
}