using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Resources;
using low_age_data.Shared;

namespace low_age_data.Collections
{
    public static class Resources
    {
        public static List<Resource> Get()
        {
            return new List<Resource>
            {
                new Resource(id: ResourceId.Scraps,
                    displayName: nameof(ResourceId.Scraps).CamelCaseToWords(),
                    description: "Used for buying units, structures, research, and anything else.",
                    sprite: "res://assets/sprites/resources/scraps indexed.png",
                    hasLimit: false,
                    isConsumable: true,
                    hasBank: true),

                new Resource(id: ResourceId.Celestium,
                    displayName: nameof(ResourceId.Celestium).CamelCaseToWords(),
                    description: "Used for production. Higher amounts of Celestium allow for quicker production of " +
                                 "units, structures, research, and anything else.",
                    sprite: "res://assets/sprites/resources/celestium indexed.png",
                    hasLimit: false,
                    isConsumable: false,
                    hasBank: false),

                new Resource(id: ResourceId.Population,
                    displayName: nameof(ResourceId.Population).CamelCaseToWords(),
                    description: "Each unit requires a population space, new units can be promoted from the " +
                                 "remaining population space. If there are more units than the amount of supported " +
                                 "population space, all units will start receiving double damage from all sources.",
                    sprite: "res://assets/sprites/resources/population indexed.png",
                    hasLimit: false,
                    isConsumable: true,
                    hasBank: false,
                    attachesToNewActors: true,
                    negativeIncomeEffects: new List<EffectId>
                    {
                        EffectId.Shared.Revelators.NoPopulationSpaceSearch
                    },
                    negativeIncomeDescription: "Caution, there are more units than the supported population."),

                new Resource(id: ResourceId.WeaponStorage,
                    displayName: nameof(ResourceId.WeaponStorage).CamelCaseToWords(),
                    description: "Maximum amount of storage for all melee, ranged and special weapons. The most " +
                                 "valuable weapons are lost if the maximum storage is decreased for any reason.",
                    sprite: "res://assets/sprites/resources/weapon all indexed.png",
                    hasLimit: false,
                    isConsumable: false,
                    hasBank: false),

                new Resource(id: ResourceId.MeleeWeapon,
                    displayName: nameof(ResourceId.MeleeWeapon).CamelCaseToWords(),
                    description: "Weapon used for promotion of units with melee qualities.",
                    sprite: "res://assets/sprites/resources/weapon melee indexed.png",
                    hasLimit: true,
                    isConsumable: true,
                    hasBank: true,
                    storedAs: ResourceId.WeaponStorage),

                new Resource(id: ResourceId.RangedWeapon,
                    displayName: nameof(ResourceId.MeleeWeapon).CamelCaseToWords(),
                    description: "Weapon used for promotion of units with ranged qualities.",
                    sprite: "res://assets/sprites/resources/weapon ranged indexed.png",
                    hasLimit: true,
                    isConsumable: true,
                    hasBank: true,
                    storedAs: ResourceId.WeaponStorage),

                new Resource(id: ResourceId.SpecialWeapon,
                    displayName: nameof(ResourceId.MeleeWeapon).CamelCaseToWords(),
                    description: "Weapon used for promotion of units with special qualities.",
                    sprite: "res://assets/sprites/resources/weapon special indexed.png",
                    hasLimit: true,
                    isConsumable: true,
                    hasBank: true,
                    storedAs: ResourceId.WeaponStorage),
                
                new Resource(id: ResourceId.Faith,
                    displayName: nameof(ResourceId.Faith).CamelCaseToWords(),
                    description: "Each point of Faith provides +1 Initiative to all owned units. Faith is generated " +
                                 "by Temples.",
                    sprite: "res://assets/sprites/resources/faith indexed.png",
                    hasLimit: false,
                    isConsumable: false,
                    hasBank: false,
                    positiveIncomeEffects: new List<EffectId>
                    {
                        EffectId.Shared.Uee.PositiveFaithSearch
                    },
                    effectAmountMultipliedByResourceAmount: true),
            };
        }
    }
}