using low_age_data.Domain.Entities;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Shared.Filters
{
    /// <summary>
    /// <see cref="Entity"/> should adhere to a specific <see cref="FilterFlag"/>.
    /// </summary>
    public class SpecificFlag : IFilterItem
    {
        public SpecificFlag(FilterFlag value)
        {
            Type = $"{nameof(SpecificFlag)}";
            Value = value;
        }
        
        public string Type { get; }
        public FilterFlag Value { get; }
    }
}