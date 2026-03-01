namespace LowAgeData.Domain.Common.Filters
{
    /// <summary>
    /// Can be used to logically group the <see cref="IFilterItem"/>s. By default, a list of <see cref="IFilterItem"/>s
    /// has the properties of a <see cref="FilterGroup"/> with <see cref="Policy"/>.<see cref="Policy.Include"/> and
    /// <see cref="Quantifier"/>.<see cref="Quantifier.All"/> values. 
    /// </summary>
    public class FilterGroup : IFilterItem
    {
        public FilterGroup(
            Policy policy, 
            Quantifier quantifier, 
            IList<IFilterItem> filters)
        {
            Policy = policy;
            Quantifier = quantifier;
            Filters = filters;
        }
        
        public Policy Policy { get; }
        public Quantifier Quantifier { get; }
        public IList<IFilterItem> Filters { get; }
    }
    
    public enum Policy
    {
        /// <summary>
        /// <see cref="IFilterItem"/> should return true.
        /// </summary>
        Include,
        
        /// <summary>
        /// <see cref="IFilterItem"/> should return false.
        /// </summary>
        Exclude
    }

    public enum Quantifier
    {
        /// <summary>
        /// Logical AND between <see cref="IFilterItem"/>s
        /// </summary>
        All,
        
        /// <summary>
        /// Logical OR between <see cref="IFilterItem"/>s. 
        /// </summary>
        Any
    }
}