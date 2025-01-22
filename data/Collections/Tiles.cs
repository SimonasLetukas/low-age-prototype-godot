using System.Collections.Generic;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Tiles;

namespace LowAgeData.Collections
{
    public static class TilesCollection
    {
        public static List<Tile> Get()
        {
            return new List<Tile>
            {
                new Tile(
                    id: TileId.Grass,
                    displayName: nameof(TileId.Grass),
                    description: "",
                    terrain: Terrain.Grass,
                    movementCost: 1.0f,
                    allowsBuilding: true),
                new Tile(
                    id: TileId.Mountains,
                    displayName: nameof(TileId.Mountains),
                    description: "",
                    terrain: Terrain.Mountains,
                    movementCost: float.PositiveInfinity,
                    allowsBuilding: false),
                new Tile(
                    id: TileId.Marsh,
                    displayName: nameof(TileId.Marsh),
                    description: "",
                    terrain: Terrain.Marsh,
                    movementCost: 2.0f,
                    allowsBuilding: false),
                new Tile(
                    id: TileId.Scraps,
                    displayName: nameof(TileId.Scraps),
                    description: "",
                    terrain: Terrain.Scraps,
                    movementCost: 1.0f,
                    allowsBuilding: true),
                new Tile(
                    id: TileId.Celestium,
                    displayName: nameof(TileId.Celestium),
                    description: "",
                    terrain: Terrain.Celestium,
                    movementCost: 1.0f,
                    allowsBuilding: true)
            };
        }
    }
}