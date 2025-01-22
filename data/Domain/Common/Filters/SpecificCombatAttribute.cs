using LowAgeData.Domain.Entities;

namespace LowAgeData.Domain.Common.Filters
{
    /// <summary>
    /// <see cref="Entity"/> should have a specific <see cref="ActorAttribute"/>.
    /// </summary>
    public class SpecificCombatAttribute : IFilterItem
    {
        public SpecificCombatAttribute(ActorAttribute value)
        {
            Value = value;
        }
        
        public ActorAttribute Value { get; }
    }
}