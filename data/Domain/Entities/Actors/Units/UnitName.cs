namespace low_age_data.Domain.Entities.Actors.Units
{
    public class UnitName : EntityName
    {
        private UnitName(string value) : base($"unit-{value}")
        {
        }

        public static UnitName Slave => new UnitName(nameof(Slave).ToLower());
        public static UnitName Leader => new UnitName(nameof(Leader).ToLower());
    }
}
