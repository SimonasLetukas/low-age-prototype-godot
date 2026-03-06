using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class UnitMovedAlongPathEvent(
    Guid entityInstanceId,
    IEnumerable<int> path)
    : IGameEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid EntityInstanceId { get; } = entityInstanceId;
    public IEnumerable<int> Path { get; } = path.ToArray();
}