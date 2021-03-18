using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class Terrains : ValueObject<Terrains>
    {
        public override string ToString()
        {
            return $"{nameof(Terrains)}.{Value}";
        }

        public static Terrains Grass => new Terrains(TerrainsEnum.Grass);
        public static Terrains Mountains => new Terrains(TerrainsEnum.Mountains);
        public static Terrains Marsh => new Terrains(TerrainsEnum.Marsh);
        public static Terrains Scraps => new Terrains(TerrainsEnum.Scraps);
        public static Terrains Celestium => new Terrains(TerrainsEnum.Celestium);

        private Terrains(TerrainsEnum @enum)
        {
            Value = @enum;
        }

        private TerrainsEnum Value { get; }

        private enum TerrainsEnum
        {
            Grass = 0,
            Mountains = 1,
            Marsh = 2,
            Scraps = 3,
            Celestium = 4
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
