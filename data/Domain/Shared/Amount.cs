using System.Collections.Generic;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Shared
{
    public class Amount
    {
        public Amount(
            int flat, 
            float? multiplier = null, 
            Flag? multiplierOf = null,
            IList<Flag>? multiplierFlags = null)
        {
            Flat = flat;
            Multiplier = multiplier;
            MultiplierOf = multiplierOf;
            MultiplierFlags = multiplierFlags ?? new List<Flag>();
        }

        public int Flat { get; }
        public float? Multiplier { get; }
        public Flag? MultiplierOf { get; }
        public IList<Flag> MultiplierFlags { get; }
    }
}
