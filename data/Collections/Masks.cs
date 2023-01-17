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
                new(name: MaskName.Power) // TODO add movespeed buff (+2?) for all friendly uee units inside power 
                
                // TODO maybe refactor the HP and ability loss of structures when not inside power -- to be applied here
                // instead of separate abilities?
            };
        }
    }
}