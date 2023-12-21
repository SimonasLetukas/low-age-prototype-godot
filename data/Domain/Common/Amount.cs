using System.Collections.Generic;
using low_age_data.Domain.Common.Filters;
using low_age_data.Domain.Common.Flags;

namespace low_age_data.Domain.Common
{
    public class Amount
    {
        public Amount(
            int flat, 
            float? multiplier = null, 
            AmountFlag? multiplierOf = null,
            IList<IFilterItem>? multiplierFilters = null)
        {
            Flat = flat;
            Multiplier = multiplier;
            MultiplierOf = multiplierOf;
            MultiplierFilters = multiplierFilters ?? new List<IFilterItem>();
        }

        public int Flat { get; }
        public float? Multiplier { get; }
        public AmountFlag? MultiplierOf { get; }
        public IList<IFilterItem> MultiplierFilters { get; }
    }
}
