using System;
using System.Collections.Generic;
using Godot;
using low_age_data.Domain.Common;
using low_age_data.Domain.Tiles;

public class MapCreatedEvent : IGameEvent
{
    public MapCreatedEvent(Vector2 mapSize, ICollection<Vector2> startingPositions, 
        ICollection<(Vector2, TileId)> tiles)
    {
        MapSize = mapSize;
        StartingPositions = startingPositions;
        Tiles = tiles;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public Vector2 MapSize { get; set; }
    public ICollection<Vector2> StartingPositions { get; set; }
    public ICollection<(Vector2, TileId)> Tiles { get; set; }
}