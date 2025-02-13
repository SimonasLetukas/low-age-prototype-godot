using System;
using System.Collections.Generic;
using LowAgeData.Domain.Tiles;
using LowAgeCommon;
using Area = LowAgeCommon.Area;

public partial class MapCreatedEvent : IGameEvent
{
    public MapCreatedEvent(Vector2Int mapSize, Dictionary<int, IList<Area>> startingPositions, 
        ICollection<(Vector2Int, TileId)> tiles)
    {
        MapSize = mapSize;
        StartingPositions = startingPositions;
        Tiles = tiles;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public Vector2Int MapSize { get; }
    public Dictionary<int, IList<Area>> StartingPositions { get; } // TODO might be better to have a
                                                                   // Dictionary<int, IList<Vector2>> for each
                                                                   // player ID instead and to use flood fill
                                                                   // algorithms to be able to have non-square
                                                                   // starting positions
    public ICollection<(Vector2Int, TileId)> Tiles { get; }
}