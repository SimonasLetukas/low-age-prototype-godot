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
                new(name: ResourceName.Scraps, 
                    displayName: nameof(ResourceName.Scraps).CamelCaseToWords(),
                    description: "",
                    hasLimit: false,
                    isConsumable: true,
                    hasBank: true),
                
                new(name: ResourceName.Celestium, 
                    displayName: nameof(ResourceName.Celestium).CamelCaseToWords(),
                    description: "",
                    hasLimit: false,
                    isConsumable: false,
                    hasBank: false),
                
                new(name: ResourceName.Population, 
                    displayName: nameof(ResourceName.Population).CamelCaseToWords(),
                    description: "",
                    hasLimit: false,
                    isConsumable: true,
                    hasBank: false,
                    attachesToNewActors: true),

                new(name: ResourceName.WeaponStorage,
                    displayName: nameof(ResourceName.WeaponStorage).CamelCaseToWords(),
                    description: "",
                    hasLimit: false,
                    isConsumable: false,
                    hasBank: false),
                
                new(name: ResourceName.MeleeWeapon, 
                    displayName: nameof(ResourceName.MeleeWeapon).CamelCaseToWords(),
                    description: "",
                    hasLimit: true,
                    isConsumable: true,
                    hasBank: true,
                    storedAs: ResourceName.WeaponStorage),
                
                new(name: ResourceName.RangedWeapon, 
                    displayName: nameof(ResourceName.MeleeWeapon).CamelCaseToWords(),
                    description: "",
                    hasLimit: true,
                    isConsumable: true,
                    hasBank: true,
                    storedAs: ResourceName.WeaponStorage),
                
                new(name: ResourceName.SpecialWeapon, 
                    displayName: nameof(ResourceName.MeleeWeapon).CamelCaseToWords(),
                    description: "",
                    hasLimit: true,
                    isConsumable: true,
                    hasBank: true,
                    storedAs: ResourceName.WeaponStorage)
            };
        }
    }
}