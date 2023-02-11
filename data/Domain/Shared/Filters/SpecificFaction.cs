using low_age_data.Domain.Entities;
using low_age_data.Domain.Factions;

namespace low_age_data.Domain.Shared.Filters
{
    /// <summary>
    /// <see cref="Entity"/> should be from a specific <see cref="Faction"/>.
    /// </summary>
    public class SpecificFaction : IFilterItem
    {
        public SpecificFaction(FactionName value)
        {
            Type = $"{nameof(SpecificFaction)}";
            Value = value;
        }
        
        public string Type { get; }
        public FactionName Value { get; }
    }
}