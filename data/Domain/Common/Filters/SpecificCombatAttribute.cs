using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Common.Filters
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