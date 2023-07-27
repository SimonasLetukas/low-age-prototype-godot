using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Shared.Filters
{
    /// <summary>
    /// <see cref="Entity"/> should have a specific <see cref="ActorAttribute"/>.
    /// </summary>
    public class SpecificCombatAttribute : IFilterItem
    {
        public SpecificCombatAttribute(ActorAttribute value)
        {
            Type = $"{nameof(SpecificCombatAttribute)}";
            Value = value;
        }
        
        public string Type { get; }
        public ActorAttribute Value { get; }
    }
}