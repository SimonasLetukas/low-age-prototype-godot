﻿using System.Collections.Generic;
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
                new(name: FactionName.Revelators,
                    displayName: nameof(FactionName.Revelators).CamelCaseToWords(),
                    description: "",
                    availableResources: new List<ResourceName>
                    {
                        ResourceName.Scraps,
                        ResourceName.Celestium,
                        ResourceName.Population,
                        ResourceName.MeleeWeapon,
                        ResourceName.RangedWeapon,
                        ResourceName.SpecialWeapon
                    },
                    startingEntities: new List<EntityName>
                    {
                        StructureName.Citadel,
                        UnitName.Leader
                    },
                    bonusStartingResources: new List<Payment>
                    {
                        new(resource: ResourceName.MeleeWeapon, amount: 4)
                    }),

                new(name: FactionName.Uee,
                    displayName: nameof(FactionName.Uee).CamelCaseToWords(),
                    description: "",
                    availableResources: new List<ResourceName>
                    {
                        ResourceName.Scraps,
                        ResourceName.Celestium,
                        ResourceName.Faith
                    },
                    startingEntities: new List<EntityName>
                    {
                        StructureName.BatteryCore
                    })
            };
        }
    }
}