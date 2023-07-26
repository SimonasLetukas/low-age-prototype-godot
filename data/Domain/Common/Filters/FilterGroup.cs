using System.Collections.Generic;

namespace low_age_data.Domain.Shared.Filters
{
    /// <summary>
    /// Can be used to logically group the <see cref="IFilterItem"/>s. By default, a list of <see cref="IFilterItem"/>s
    /// has the properties of a <see cref="FilterGroup"/> with <see cref="Policy"/>.Include and
    /// <see cref="Quantifier"/>.All values. 
    /// </summary>
    public class FilterGroup : IFilterItem
    {
        public FilterGroup(
            Policy policy, 
            Quantifier quantifier, 
            IList<IFilterItem> items)
        {
            Type = $"{nameof(FilterGroup)}";
            Policy = policy;
            Quantifier = quantifier;
            Items = items;
        }
        
        public string Type { get; }
        public Policy Policy { get; }
        public Quantifier Quantifier { get; }
        public IList<IFilterItem> Items { get; }
    }
    
    public enum Policy
    {
        Include,
        Exclude
    }

    public enum Quantifier
    {
        All,
        Any
    }
}