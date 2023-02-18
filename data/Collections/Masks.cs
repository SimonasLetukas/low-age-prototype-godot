using System.Collections.Generic;
using low_age_data.Domain.Masks;

namespace low_age_data.Collections
{
    public static class Masks
    {
        public static List<Mask> Get()
        {
            return new List<Mask>
            {
                new Mask(name: MaskName.Power)
            };
        }
    }
}