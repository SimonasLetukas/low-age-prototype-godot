using low_age_data.Common;

namespace low_age_data.Domain.Entities.Actors.Units
{
    public class UnitId : EntityId
    {
        private UnitId(string value) : base($"unit-{value}")
        {
        }

        public static UnitId Slave => new UnitId(nameof(Slave).ToKebabCase());
        public static UnitId Leader => new UnitId(nameof(Leader).ToKebabCase());
        public static UnitId Quickdraw => new UnitId(nameof(Quickdraw).ToKebabCase());
        public static UnitId Gorger => new UnitId(nameof(Gorger).ToKebabCase());
        public static UnitId Camou => new UnitId(nameof(Camou).ToKebabCase());
        public static UnitId Shaman => new UnitId(nameof(Shaman).ToKebabCase());
        public static UnitId Pyre => new UnitId(nameof(Pyre).ToKebabCase());
        public static UnitId BigBadBull => new UnitId(nameof(BigBadBull).ToKebabCase());
        public static UnitId Mummy => new UnitId(nameof(Mummy).ToKebabCase());
        public static UnitId Roach => new UnitId(nameof(Roach).ToKebabCase());
        public static UnitId Parasite => new UnitId(nameof(Parasite).ToKebabCase());

        public static UnitId Horrior => new UnitId(nameof(Horrior).ToKebabCase());
        public static UnitId Surfer => new UnitId(nameof(Surfer).ToKebabCase());
        public static UnitId Marksman => new UnitId(nameof(Marksman).ToKebabCase());
        public static UnitId Mortar => new UnitId(nameof(Mortar).ToKebabCase());
        public static UnitId Hawk => new UnitId(nameof(Hawk).ToKebabCase());
        public static UnitId Engineer => new UnitId(nameof(Engineer).ToKebabCase());
        public static UnitId Cannon => new UnitId(nameof(Cannon).ToKebabCase());
        public static UnitId Ballista => new UnitId(nameof(Ballista).ToKebabCase());
        public static UnitId Radar => new UnitId(nameof(Radar).ToKebabCase());
        public static UnitId Vessel => new UnitId(nameof(Vessel).ToKebabCase());
        public static UnitId Omen => new UnitId(nameof(Omen).ToKebabCase());
    }
}
