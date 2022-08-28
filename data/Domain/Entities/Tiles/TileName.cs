namespace low_age_data.Domain.Entities.Tiles
{
    public class TileName : EntityName
    {
        private TileName(string value) : base($"tile-{value}")
        {
        }

        public static TileName Grass => new(nameof(Grass).ToLower());
        public static TileName Mountains => new(nameof(Mountains).ToLower());
        public static TileName Marsh => new(nameof(Marsh).ToLower());
        public static TileName Scraps => new(nameof(Scraps).ToLower());
        public static TileName Celestium => new(nameof(Celestium).ToLower());
    }
}
