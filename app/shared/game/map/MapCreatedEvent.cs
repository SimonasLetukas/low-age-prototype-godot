using System;
using System.Collections.Generic;
using Godot;
using low_age_data.Domain.Tiles;

public class MapCreatedEvent : IGameEvent
{
    public MapCreatedEvent(Vector2 mapSize, Dictionary<int, IList<Rect2>> startingPositions, 
        ICollection<(Vector2, TileId)> tiles)
    {
        MapSize = mapSize;
        StartingPositions = startingPositions;
        Tiles = tiles;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public Vector2 MapSize { get; set; }
    public Dictionary<int, IList<Rect2>> StartingPositions { get; set; } // TODO might be better to have a
                                                                         // Dictionary<int, IList<Vector2>> for each
                                                                         // player ID instead and to use flood fill
                                                                         // algorithms to be able to have non-square
                                                                         // starting positions
    public ICollection<(Vector2, TileId)> Tiles { get; set; }
}