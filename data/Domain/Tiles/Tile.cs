using LowAgeData.Domain.Common;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Tiles
{
    public class Tile
    {
        public Tile(
            TileId id, 
            string displayName, 
            string description, 
            Terrain terrain, 
            float movementCost, 
            bool allowsBuilding)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Terrain = terrain;
            MovementCost = movementCost;
            AllowsBuilding = allowsBuilding;
        }

        [JsonProperty(Order = -3)]
        public TileId Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public Terrain Terrain { get; }
        public float MovementCost { get; }
        public bool AllowsBuilding { get; }
    }
}
