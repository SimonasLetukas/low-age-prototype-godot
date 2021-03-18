using System.Collections.Generic;
using low_age_data.Domain.Entities.Tiles;
using low_age_data.Domain.Shared;

namespace low_age_data.Collections
{
    public static class Tiles
    {
        public static List<Tile> Get()
        {
            return new List<Tile>
            {
                new Tile(
                    TileName.Grass,
                    nameof(TileName.Grass),
                    "",
                    Terrains.Grass,
                    1.0f,
                    true),
                new Tile(
                    TileName.Mountains,
                    nameof(TileName.Mountains),
                    "",
                    Terrains.Mountains,
                    0.0f,
                    false),
                new Tile(
                    TileName.Marsh,
                    nameof(TileName.Marsh),
                    "",
                    Terrains.Marsh,
                    2.0f,
                    false),
                new Tile(
                    TileName.Scraps,
                    nameof(TileName.Scraps),
                    "",
                    Terrains.Scraps,
                    1.0f,
                    true),
                new Tile(
                    TileName.Celestium,
                    nameof(TileName.Celestium),
                    "",
                    Terrains.Celestium,
                    1.0f,
                    true)
            };
        }
    }
}
