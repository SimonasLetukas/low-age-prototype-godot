using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Entities.Actors.Structures;
using LowAgeData.Domain.Entities.Actors.Units;
using LowAgeData.Domain.Factions;
using LowAgeData.Domain.Resources;
using LowAgeCommon;

namespace LowAgeData.Collections
{
    public static class FactionsCollection
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
                        UnitId.Slave,
                        UnitId.Slave,
                        UnitId.Leader,
                        StructureId.Citadel,
                    },
                    bonusStartingResources: new List<Payment>
                    {
                        new Payment(resource: ResourceId.MeleeWeapon, amount: 4)
                    }),

                new Faction(id: FactionId.Uee,
                    displayName: nameof(FactionId.Uee).CamelCaseToWords().ToUpper(),
                    description: "",
                    availableResources: new List<ResourceId>
                    {
                        ResourceId.Scraps,
                        ResourceId.Celestium,
                        ResourceId.Faith
                    },
                    startingEntities: new List<EntityId>
                    {
                        UnitId.Marksman,
                        UnitId.Horrior,
                        StructureId.BatteryCore
                    })
            };
        }
    }
}