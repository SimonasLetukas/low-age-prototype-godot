using low_age_data.Common;

namespace low_age_data.Domain.Factions
{
    public class FactionName : Name
    {
        private FactionName(string value) : base($"faction-{value}")
        {
        }

        public static FactionName Uee => new($"{nameof(Uee)}".ToKebabCase());
        public static FactionName Revelators => new($"{nameof(Revelators)}".ToKebabCase());
    }
}