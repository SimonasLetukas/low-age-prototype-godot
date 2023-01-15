using low_age_data.Common;

namespace low_age_data.Domain.Entities.Actors.Structures
{
    public class StructureName : EntityName
    {
        private StructureName(string value) : base($"structure-{value}")
        {
        }

        public static StructureName Citadel => new(nameof(Citadel).ToKebabCase());
        public static StructureName Hut => new(nameof(Hut).ToKebabCase());
        public static StructureName Obelisk => new(nameof(Obelisk).ToKebabCase());
        public static StructureName Shack => new(nameof(Shack).ToKebabCase());
        public static StructureName Smith => new(nameof(Smith).ToKebabCase());
        public static StructureName Fletcher => new(nameof(Fletcher).ToKebabCase());
        public static StructureName Alchemy => new(nameof(Alchemy).ToKebabCase());
        public static StructureName Depot => new(nameof(Depot).ToKebabCase());
        public static StructureName Workshop => new(nameof(Workshop).ToKebabCase());
        public static StructureName Outpost => new(nameof(Outpost).ToKebabCase());
        public static StructureName Barricade => new(nameof(Barricade).ToKebabCase());

        public static StructureName BatteryCore => new(nameof(BatteryCore).ToKebabCase());
        public static StructureName FusionCore => new(nameof(FusionCore).ToKebabCase());
        public static StructureName CelestiumCore => new(nameof(CelestiumCore).ToKebabCase());
    }
}
