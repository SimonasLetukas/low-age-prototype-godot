using low_age_data.Common;

namespace low_age_data.Domain.Entities.Actors.Structures
{
    public class StructureName : EntityName
    {
        private StructureName(string value) : base($"structure-{value}")
        {
        }

        public static StructureName Citadel => new StructureName(nameof(Citadel).ToKebabCase());
        public static StructureName Hut => new StructureName(nameof(Hut).ToKebabCase());
        public static StructureName Obelisk => new StructureName(nameof(Obelisk).ToKebabCase());
        public static StructureName Shack => new StructureName(nameof(Shack).ToKebabCase());
        public static StructureName Smith => new StructureName(nameof(Smith).ToKebabCase());
        public static StructureName Fletcher => new StructureName(nameof(Fletcher).ToKebabCase());
        public static StructureName Alchemy => new StructureName(nameof(Alchemy).ToKebabCase());
        public static StructureName Depot => new StructureName(nameof(Depot).ToKebabCase());
        public static StructureName Workshop => new StructureName(nameof(Workshop).ToKebabCase());
        public static StructureName Outpost => new StructureName(nameof(Outpost).ToKebabCase());
        public static StructureName Barricade => new StructureName(nameof(Barricade).ToKebabCase());

        public static StructureName BatteryCore => new StructureName(nameof(BatteryCore).ToKebabCase());
        public static StructureName FusionCore => new StructureName(nameof(FusionCore).ToKebabCase());
        public static StructureName CelestiumCore => new StructureName(nameof(CelestiumCore).ToKebabCase());
        public static StructureName Collector => new StructureName(nameof(Collector).ToKebabCase());
        public static StructureName Extractor => new StructureName(nameof(Extractor).ToKebabCase());
        public static StructureName PowerPole => new StructureName(nameof(PowerPole).ToKebabCase());
        public static StructureName Temple => new StructureName(nameof(Temple).ToKebabCase());
        public static StructureName MilitaryBase => new StructureName(nameof(MilitaryBase).ToKebabCase());
        public static StructureName Factory => new StructureName(nameof(Factory).ToKebabCase());
        public static StructureName Laboratory => new StructureName(nameof(Laboratory).ToKebabCase());
        public static StructureName Armoury => new StructureName(nameof(Armoury).ToKebabCase());
        public static StructureName Wall => new StructureName(nameof(Wall).ToKebabCase());
        public static StructureName Stairs => new StructureName(nameof(Stairs).ToKebabCase());
        public static StructureName Gate => new StructureName(nameof(Gate).ToKebabCase());
        public static StructureName Watchtower => new StructureName(nameof(Watchtower).ToKebabCase());
        public static StructureName Bastion => new StructureName(nameof(Bastion).ToKebabCase());
    }
}
