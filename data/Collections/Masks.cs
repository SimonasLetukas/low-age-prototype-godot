﻿using System.Collections.Generic;
using low_age_data.Domain.Masks;

namespace low_age_data.Collections
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