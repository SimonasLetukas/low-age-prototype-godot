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
                    name: TileName.Grass,
                    displayName: nameof(TileName.Grass),
                    description: "",
                    terrain: Terrains.Grass,
                    movementCost: 1.0f,
                    allowsBuilding: true),
                new Tile(
                    name: TileName.Mountains,
                    displayName: nameof(TileName.Mountains),
                    description: "",
                    terrain: Terrains.Mountains,
                    movementCost: 0.0f,
                    allowsBuilding: false),
                new Tile(
                    name: TileName.Marsh,
                    displayName: nameof(TileName.Marsh),
                    description: "",
                    terrain: Terrains.Marsh,
                    movementCost: 2.0f,
                    allowsBuilding: false),
                new Tile(
                    name: TileName.Scraps,
                    displayName: nameof(TileName.Scraps),
                    description: "",
                    terrain: Terrains.Scraps,
                    movementCost: 1.0f,
                    allowsBuilding: true),
                new Tile(
                    name: TileName.Celestium,
                    displayName: nameof(TileName.Celestium),
                    description: "",
                    terrain: Terrains.Celestium,
                    movementCost: 1.0f,
                    allowsBuilding: true)
            };
        }
    }
}