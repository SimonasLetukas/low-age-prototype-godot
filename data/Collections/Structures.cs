using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Entities.Actors.Structures;
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
                    originalFaction: Factions.Revelators, 
                    combatAttributes: new List<CombatAttributes>(),
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Citadel.PassiveIncome
                        // TODO
                    },
                    size: new Vector2<int>(x: 3, y: 4),
                    centerPoint: new Vector2<int>(x: 1, y: 1),
                    destructible: false,
                    startingStructure: true,
                    walkableArea: new Area(
                        start: new Vector2<int>(x: 0, y: 2), 
                        size: new Vector2<int>(x: 2, y: 2)))
            };
        }
    }
}
