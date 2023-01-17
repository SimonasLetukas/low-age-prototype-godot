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
        public static StructureName Collector => new(nameof(Collector).ToKebabCase());
        public static StructureName Extractor => new(nameof(Extractor).ToKebabCase());
        public static StructureName PowerPole => new(nameof(PowerPole).ToKebabCase());
        public static StructureName Temple => new(nameof(Temple).ToKebabCase());
        public static StructureName MilitaryBase => new(nameof(MilitaryBase).ToKebabCase());
        public static StructureName Factory => new(nameof(Factory).ToKebabCase());
        public static StructureName Laboratory => new(nameof(Laboratory).ToKebabCase());
        public static StructureName Armoury => new(nameof(Armoury).ToKebabCase());
        public static StructureName Wall => new(nameof(Wall).ToKebabCase());
        public static StructureName Stairs => new(nameof(Stairs).ToKebabCase());
        public static StructureName Gate => new(nameof(Gate).ToKebabCase());
        public static StructureName Watchtower => new(nameof(Watchtower).ToKebabCase());
        public static StructureName Bastion => new(nameof(Bastion).ToKebabCase());
    }
}
