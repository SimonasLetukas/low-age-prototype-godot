using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Shared.Filters
{
    /// <summary>
    /// <see cref="Entity"/> should have a specific <see cref="CombatAttribute"/>.
    /// </summary>
    public class SpecificCombatAttribute : IFilterItem
    {
        public SpecificCombatAttribute(CombatAttribute value)
        {
            Type = $"{nameof(SpecificCombatAttribute)}";
            Value = value;
        }
        
        public string Type { get; }
        public CombatAttribute Value { get; }
    }
}