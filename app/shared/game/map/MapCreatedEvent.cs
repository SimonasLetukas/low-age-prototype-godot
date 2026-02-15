using System;
using System.Collections.Generic;
using LowAgeData.Domain.Tiles;
using LowAgeCommon;
using Area = LowAgeCommon.Area;

public partial class MapCreatedEvent : IGameEvent
{
    public MapCreatedEvent(string mapLocation, Vector2Int mapSize, 
        Dictionary<int, Area> startingPositions, ICollection<(Vector2Int, TileId)> tiles)
    {
        MapLocation = mapLocation;
        MapSize = mapSize;
        StartingPositions = startingPositions;
        Tiles = tiles;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public string MapLocation { get; init; }
    public Vector2Int MapSize { get; }
    public Dictionary<int, Area> StartingPositions { get; } 
    public ICollection<(Vector2Int, TileId)> Tiles { get; }
}