using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Entities.Tiles
{
    public class Tile : Entity
    {
        public Tile(
            TileId id, 
            string displayName, 
            string description, 
            Terrain terrain, 
            float movementCost, 
            bool allowsBuilding) : base(id, displayName, description)
        {
            Terrain = terrain;
            MovementCost = movementCost;
            AllowsBuilding = allowsBuilding;
        }

        public Terrain Terrain { get; }
        public float MovementCost { get; }
        public bool AllowsBuilding { get; }
    }
}
