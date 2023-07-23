namespace low_age_data.Domain.Entities.Tiles
{
    public class TileId : EntityId
    {
        private TileId(string value) : base($"tile-{value}")
        {
        }

        public static TileId Grass => new TileId(nameof(Grass).ToLower());
        public static TileId Mountains => new TileId(nameof(Mountains).ToLower());
        public static TileId Marsh => new TileId(nameof(Marsh).ToLower());
        public static TileId Scraps => new TileId(nameof(Scraps).ToLower());
        public static TileId Celestium => new TileId(nameof(Celestium).ToLower());
    }
}
