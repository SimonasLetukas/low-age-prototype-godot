using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Effects;
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
                    description: "", // TODO
                    hasLimit: false,
                    isConsumable: true,
                    hasBank: true),
                
                new(name: ResourceName.Celestium, 
                    displayName: nameof(ResourceName.Celestium).CamelCaseToWords(),
                    description: "", // TODO
                    hasLimit: false,
                    isConsumable: false,
                    hasBank: false),
                
                new(name: ResourceName.Population, 
                    displayName: nameof(ResourceName.Population).CamelCaseToWords(),
                    description: "Each unit requires a population space, new units can be promoted from the " +
                                 "remaining population space. If there are more units than the amount of supported " +
                                 "population space, all units will start receiving double damage from all sources.", 
                    hasLimit: false,
                    isConsumable: true,
                    hasBank: false,
                    attachesToNewActors: true,
                    negativeIncomeEffects: new List<EffectName>
                    {
                        EffectName.Shared.NoPopulationSpaceSearch
                    },
                    negativeIncomeDescription: "Caution, there are more units than the supported population."),

                new(name: ResourceName.WeaponStorage,
                    displayName: nameof(ResourceName.WeaponStorage).CamelCaseToWords(),
                    description: "", // TODO
                    hasLimit: false,
                    isConsumable: false,
                    hasBank: false),
                
                new(name: ResourceName.MeleeWeapon, 
                    displayName: nameof(ResourceName.MeleeWeapon).CamelCaseToWords(),
                    description: "", // TODO
                    hasLimit: true,
                    isConsumable: true,
                    hasBank: true,
                    storedAs: ResourceName.WeaponStorage),
                
                new(name: ResourceName.RangedWeapon, 
                    displayName: nameof(ResourceName.MeleeWeapon).CamelCaseToWords(),
                    description: "", // TODO
                    hasLimit: true,
                    isConsumable: true,
                    hasBank: true,
                    storedAs: ResourceName.WeaponStorage),
                
                new(name: ResourceName.SpecialWeapon, 
                    displayName: nameof(ResourceName.MeleeWeapon).CamelCaseToWords(),
                    description: "", // TODO
                    hasLimit: true,
                    isConsumable: true,
                    hasBank: true,
                    storedAs: ResourceName.WeaponStorage)
            };
        }
    }
}