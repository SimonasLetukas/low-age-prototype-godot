using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Factions;
using low_age_data.Domain.Shared;

namespace low_age_data.Collections
{
    public static class Structures
    {
        public static List<Structure> Get()
        {
            return new List<Structure>
            {
                new(
                    name: StructureName.Citadel, 
                    displayName: nameof(StructureName.Citadel).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators, 
                    combatAttributes: new List<CombatAttributes>(),
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Citadel.PassiveIncome,
                        AbilityName.Citadel.ExecutiveStash,
                        AbilityName.Citadel.Ascendable,
                        AbilityName.Citadel.HighGround,
                        AbilityName.Citadel.PromoteGoons
                    },
                    size: new Vector2<int>(x: 3, y: 4),
                    centerPoint: new Vector2<int>(x: 1, y: 1),
                    destructible: false,
                    walkableArea: new Area(
                        start: new Vector2<int>(x: 0, y: 2), 
                        size: new Vector2<int>(x: 3, y: 2))),
                
                new(
                    name: StructureName.Hut, 
                    displayName: nameof(StructureName.Hut).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 50, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators, 
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Hut.Building, // TODO add to all buildings
                        AbilityName.Hut.ScrapsIncome
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
            };
        }
    }
}
