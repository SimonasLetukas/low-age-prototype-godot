using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Entities.Actors;
using LowAgeData.Domain.Factions;

namespace LowAgeData.Domain.Common.Filters
{
    /// <summary>
    /// <see cref="Entity"/> blueprint should originally be from a specific <see cref="Faction"/> (checks
    /// <see cref="Actor.OriginalFaction"/>).
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