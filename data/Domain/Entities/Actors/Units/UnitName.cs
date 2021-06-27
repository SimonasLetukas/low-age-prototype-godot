using low_age_data.Common;

namespace low_age_data.Domain.Entities.Actors.Units
{
    public class UnitName : EntityName
    {
        private UnitName(string value) : base($"unit-{value}")
        {
        }

        public static UnitName Slave => new UnitName(nameof(Slave).ToKebabCase());
        public static UnitName Leader => new UnitName(nameof(Leader).ToKebabCase());
        public static UnitName Quickdraw => new UnitName(nameof(Quickdraw).ToKebabCase());
        public static UnitName Gorger => new UnitName(nameof(Gorger).ToKebabCase());
        public static UnitName Camou => new UnitName(nameof(Camou).ToKebabCase());
        public static UnitName Shaman => new UnitName(nameof(Shaman).ToKebabCase());
        public static UnitName Pyre => new UnitName(nameof(Pyre).ToKebabCase());
        public static UnitName BigBadBull => new UnitName(nameof(BigBadBull).ToKebabCase());
        public static UnitName Mummy => new UnitName(nameof(Mummy).ToKebabCase());
        public static UnitName Roach => new UnitName(nameof(Roach).ToKebabCase());
        public static UnitName Parasite => new UnitName(nameof(Parasite).ToKebabCase());

        public static UnitName Horrior => new UnitName(nameof(Horrior).ToKebabCase());
        public static UnitName Surfer => new UnitName(nameof(Surfer).ToKebabCase());
        public static UnitName Marksman => new UnitName(nameof(Marksman).ToKebabCase());
        public static UnitName Mortar => new UnitName(nameof(Mortar).ToKebabCase());
        public static UnitName Hawk => new UnitName(nameof(Hawk).ToKebabCase());
        public static UnitName Engineer => new UnitName(nameof(Engineer).ToKebabCase());
        public static UnitName Cannon => new UnitName(nameof(Cannon).ToKebabCase());
        public static UnitName Ballista => new UnitName(nameof(Ballista).ToKebabCase());
        public static UnitName Radar => new UnitName(nameof(Radar).ToKebabCase());
        public static UnitName Vessel => new UnitName(nameof(Vessel).ToKebabCase());
        public static UnitName Omen => new UnitName(nameof(Omen).ToKebabCase());
    }
}
