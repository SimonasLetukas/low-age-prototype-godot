using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Entities;

namespace LowAgeData.Domain.Common.Filters
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