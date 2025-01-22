using System.Collections.Generic;
using LowAgeData.Domain.Masks;

namespace LowAgeData.Collections
{
    public static class MasksCollection
    {
        public static List<Mask> Get()
        {
            return new List<Mask>
            {
                new Mask(id: MaskId.Power)
            };
        }
    }
}