using low_age_data.Common;

namespace low_age_data.Domain.Entities.Actors.Units
{
    public class UnitName : EntityName
    {
        private UnitName(string value) : base($"unit-{value}")
        {
        }

        public static UnitName Slave => new(nameof(Slave).ToKebabCase());
        public static UnitName Leader => new(nameof(Leader).ToKebabCase());
        public static UnitName Quickdraw => new(nameof(Quickdraw).ToKebabCase());
        public static UnitName Gorger => new(nameof(Gorger).ToKebabCase());
        public static UnitName Camou => new(nameof(Camou).ToKebabCase());
        public static UnitName Shaman => new(nameof(Shaman).ToKebabCase());
        public static UnitName Pyre => new(nameof(Pyre).ToKebabCase());
        public static UnitName BigBadBull => new(nameof(BigBadBull).ToKebabCase());
        public static UnitName Mummy => new(nameof(Mummy).ToKebabCase());
        public static UnitName Roach => new(nameof(Roach).ToKebabCase());
        public static UnitName Parasite => new(nameof(Parasite).ToKebabCase());

        public static UnitName Horrior => new(nameof(Horrior).ToKebabCase());
        public static UnitName Surfer => new(nameof(Surfer).ToKebabCase());
        public static UnitName Marksman => new(nameof(Marksman).ToKebabCase());
        public static UnitName Mortar => new(nameof(Mortar).ToKebabCase());
        public static UnitName Hawk => new(nameof(Hawk).ToKebabCase());
        public static UnitName Engineer => new(nameof(Engineer).ToKebabCase());
        public static UnitName Cannon => new(nameof(Cannon).ToKebabCase());
        public static UnitName Ballista => new(nameof(Ballista).ToKebabCase());
        public static UnitName Radar => new(nameof(Radar).ToKebabCase());
        public static UnitName Vessel => new(nameof(Vessel).ToKebabCase());
        public static UnitName Omen => new(nameof(Omen).ToKebabCase());
    }
}
