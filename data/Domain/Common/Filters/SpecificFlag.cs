using low_age_data.Domain.Common.Flags;
using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Common.Filters
{
    /// <summary>
    /// <see cref="Entity"/> should adhere to a specific <see cref="FilterFlag"/>.
    /// </summary>
    public class SpecificFlag : IFilterItem
    {
        public SpecificFlag(FilterFlag value)
        {
            Value = value;
        }
        
        public FilterFlag Value { get; }
    }
}