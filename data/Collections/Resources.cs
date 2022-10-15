using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Resources;

namespace low_age_data.Collections
{
    public static class Resources
    {
        public static List<Resource> Get()
        {
            return new List<Resource>
            {
                new(ResourceName.Scraps, 
                    nameof(ResourceName.Scraps).CamelCaseToWords(),
                    "",
                    false,
                    true,
                    true),
                
                new(ResourceName.Celestium, 
                    nameof(ResourceName.Celestium).CamelCaseToWords(),
                    "",
                    false,
                    false,
                    false),
                
                new(ResourceName.Population, 
                    nameof(ResourceName.Population).CamelCaseToWords(),
                    "",
                    true,
                    true,
                    false),
                
                new(ResourceName.MeleeWeapon, 
                    nameof(ResourceName.MeleeWeapon).CamelCaseToWords(),
                    "",
                    true,
                    true,
                    true),
                
                new(ResourceName.RangedWeapon, 
                    nameof(ResourceName.MeleeWeapon).CamelCaseToWords(),
                    "",
                    true,
                    true,
                    true),
                
                new(ResourceName.SpecialWeapon, 
                    nameof(ResourceName.MeleeWeapon).CamelCaseToWords(),
                    "",
                    true,
                    true,
                    true)
            };
        }
    }
}