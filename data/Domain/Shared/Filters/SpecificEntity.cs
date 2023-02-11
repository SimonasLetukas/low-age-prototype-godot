using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Shared.Filters
{
    /// <summary>
    /// Should be a specific <see cref="Entity"/>.
    /// </summary>
    public class SpecificEntity : IFilterItem
    {
        public SpecificEntity(EntityName value)
        {
            Type = $"{nameof(SpecificEntity)}";
            Value = value;
        }
        
        public string Type { get; }
        public EntityName Value { get; }
    }
}