﻿using System;
using System.Collections.Generic;
using low_age_data.Domain.Tiles;
using low_age_prototype_common;
using Area = low_age_prototype_common.Area;

public class MapCreatedEvent : IGameEvent
{
    public MapCreatedEvent(Vector2<int> mapSize, Dictionary<int, IList<Area>> startingPositions, 
        ICollection<(Vector2<int>, TileId)> tiles)
    {
        MapSize = mapSize;
        StartingPositions = startingPositions;
        Tiles = tiles;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public Vector2<int> MapSize { get; }
    public Dictionary<int, IList<Area>> StartingPositions { get; } // TODO might be better to have a
                                                                   // Dictionary<int, IList<Vector2>> for each
                                                                   // player ID instead and to use flood fill
                                                                   // algorithms to be able to have non-square
                                                                   // starting positions
    public ICollection<(Vector2<int>, TileId)> Tiles { get; }
}