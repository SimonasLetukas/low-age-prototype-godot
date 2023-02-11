using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Factions;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Entities.Actors.Units
{
    public class Unit : Actor
    {
        public Unit(
            UnitName name, 
            string displayName, 
            string description, 
            IList<Stat> statistics, 
            FactionName originalFaction, 
            IList<CombatAttribute> combatAttributes, 
            IList<AbilityName> abilities,
            int? size = null) : base(name, displayName, description, statistics, originalFaction, combatAttributes, abilities)
        {
            Size = size ?? 1;
        }

        public int Size { get; }
    }
}
