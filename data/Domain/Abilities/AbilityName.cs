namespace low_age_data.Domain.Abilities
{
    public class AbilityName : Name
    {
        private AbilityName(string value) : base($"ability-{value}")
        {
        }

        public static class Slave
        {
            public static AbilityName Build => new AbilityName("slave-build");
            public static AbilityName Repair => new AbilityName("slave-repair");
            public static AbilityName ManualLabour => new AbilityName("slave-manual-labour");
        }
    }
}
