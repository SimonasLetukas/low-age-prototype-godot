using low_age_data.Common;

namespace low_age_data.Domain.Abilities
{
    public class AbilityName : Name
    {
        private AbilityName(string value) : base($"ability-{value}")
        {
        }

        public static class Leader
        {
            public static AbilityName AllForOne => new AbilityName($"{nameof(Leader)}{nameof(AllForOne)}".ToKebabCase());
            public static AbilityName MenacingPresence => new AbilityName($"{nameof(Leader)}{nameof(MenacingPresence)}".ToKebabCase());
            public static AbilityName OneForAll => new AbilityName($"{nameof(Leader)}{nameof(OneForAll)}".ToKebabCase());
        }

        public static class Slave
        {
            public static AbilityName Build => new AbilityName($"{nameof(Slave)}{nameof(Build)}".ToKebabCase());
            public static AbilityName Repair => new AbilityName($"{nameof(Slave)}{nameof(Repair)}".ToKebabCase());
            public static AbilityName ManualLabour => new AbilityName($"{nameof(Slave)}{nameof(ManualLabour)}".ToKebabCase());
        }
    }
}
