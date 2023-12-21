using low_age_data.Domain.Entities;
using low_age_data.Domain.Factions;

namespace low_age_data.Domain.Common.Filters
{
    /// <summary>
    /// <see cref="Entity"/> should be from a specific <see cref="Faction"/>.
    /// </summary>
    public class SpecificFaction : IFilterItem
    {
        public SpecificFaction(FactionId value)
        {
            Value = value;
        }
        
        public FactionId Value { get; }
    }
}