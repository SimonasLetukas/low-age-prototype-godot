using LowAgeData.Domain.Common;
using LowAgeData.Domain.Tiles;
using LowAgeCommon;

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
                    movementCost: 1.0f),
                new Tile(
                    id: TileId.Mountains,
                    displayName: nameof(TileId.Mountains),
                    description: "",
                    terrain: Terrain.Mountains,
                    movementCost: float.PositiveInfinity),
                new Tile(
                    id: TileId.Marsh,
                    displayName: nameof(TileId.Marsh),
                    description: "",
                    terrain: Terrain.Marsh,
                    movementCost: 2.0f),
                new Tile(
                    id: TileId.Scraps,
                    displayName: nameof(TileId.Scraps),
                    description: "",
                    terrain: Terrain.Scraps,
                    movementCost: 1.0f),
                new Tile(
                    id: TileId.Celestium,
                    displayName: nameof(TileId.Celestium),
                    description: "",
                    terrain: Terrain.Celestium,
                    movementCost: 1.0f),
                new Tile(
                    id: TileId.HighGround,
                    displayName: nameof(TileId.HighGround).CamelCaseToWords(),
                    description: "",
                    terrain: Terrain.HighGround,
                    movementCost: 1.0f),
            };
        }
    }
}