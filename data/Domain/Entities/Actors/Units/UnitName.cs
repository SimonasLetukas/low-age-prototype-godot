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
    }
}
