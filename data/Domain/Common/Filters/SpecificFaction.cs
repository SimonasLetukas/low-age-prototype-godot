using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Factions;

namespace LowAgeData.Domain.Common.Filters
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