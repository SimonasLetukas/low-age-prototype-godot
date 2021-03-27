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
    }
}
