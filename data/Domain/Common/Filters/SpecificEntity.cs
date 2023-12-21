using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Common.Filters
{
    /// <summary>
    /// Should be a specific <see cref="Entity"/>.
    /// </summary>
    public class SpecificEntity : IFilterItem
    {
        public SpecificEntity(EntityId value)
        {
            Value = value;
        }
        
        public EntityId Value { get; }
    }
}