namespace low_age_data.Domain.Entities.Tiles
{
    public class TileName : EntityName
    {
        private TileName(string value) : base($"tile-{value}")
        {
        }

        public static TileName Grass => new TileName(nameof(Grass).ToLower());
        public static TileName Mountains => new TileName(nameof(Mountains).ToLower());
        public static TileName Marsh => new TileName(nameof(Marsh).ToLower());
        public static TileName Scraps => new TileName(nameof(Scraps).ToLower());
        public static TileName Celestium => new TileName(nameof(Celestium).ToLower());
    }
}
