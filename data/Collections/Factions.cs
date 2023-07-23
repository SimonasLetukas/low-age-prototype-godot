using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Factions;
using low_age_data.Domain.Resources;
using low_age_data.Domain.Shared;

namespace low_age_data.Collections
{
    public static class Factions
    {
        public static List<Faction> Get()
        {
            return new List<Faction>
            {
                new Faction(id: FactionId.Revelators,
                    displayName: nameof(FactionId.Revelators).CamelCaseToWords(),
                    description: "",
                    availableResources: new List<ResourceId>
                    {
                        ResourceId.Scraps,
                        ResourceId.Celestium,
                        ResourceId.Population,
                        ResourceId.MeleeWeapon,
                        ResourceId.RangedWeapon,
                        ResourceId.SpecialWeapon
                    },
                    startingEntities: new List<EntityId>
                    {
                        StructureId.Citadel,
                        UnitId.Leader
                    },
                    bonusStartingResources: new List<Payment>
                    {
                        new Payment(resource: ResourceId.MeleeWeapon, amount: 4)
                    }),

                new Faction(id: FactionId.Uee,
                    displayName: nameof(FactionId.Uee).CamelCaseToWords(),
                    description: "",
                    availableResources: new List<ResourceId>
                    {
                        ResourceId.Scraps,
                        ResourceId.Celestium,
                        ResourceId.Faith
                    },
                    startingEntities: new List<EntityId>
                    {
                        StructureId.BatteryCore
                    })
            };
        }
    }
}