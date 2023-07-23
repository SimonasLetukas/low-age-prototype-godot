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
                    id: TileId.Grass,
                    displayName: nameof(TileId.Grass),
                    description: "",
                    terrain: Terrains.Grass,
                    movementCost: 1.0f,
                    allowsBuilding: true),
                new Tile(
                    id: TileId.Mountains,
                    displayName: nameof(TileId.Mountains),
                    description: "",
                    terrain: Terrains.Mountains,
                    movementCost: 0.0f,
                    allowsBuilding: false),
                new Tile(
                    id: TileId.Marsh,
                    displayName: nameof(TileId.Marsh),
                    description: "",
                    terrain: Terrains.Marsh,
                    movementCost: 2.0f,
                    allowsBuilding: false),
                new Tile(
                    id: TileId.Scraps,
                    displayName: nameof(TileId.Scraps),
                    description: "",
                    terrain: Terrains.Scraps,
                    movementCost: 1.0f,
                    allowsBuilding: true),
                new Tile(
                    id: TileId.Celestium,
                    displayName: nameof(TileId.Celestium),
                    description: "",
                    terrain: Terrains.Celestium,
                    movementCost: 1.0f,
                    allowsBuilding: true)
            };
        }
    }
}