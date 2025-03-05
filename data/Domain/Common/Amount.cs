using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Common.Flags;

namespace LowAgeData.Domain.Common
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
