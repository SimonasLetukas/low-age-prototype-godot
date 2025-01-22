using LowAgeData.Domain.Entities;

namespace LowAgeData.Domain.Common.Filters
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