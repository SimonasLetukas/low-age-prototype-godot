using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Entities.Actors.Structures
{
    public class Structure : Actor
    {
        public Structure(
            EntityName name, 
            string displayName, 
            string description, 
            IList<Stat> statistics, 
            Factions originalFaction, 
            IList<CombatAttributes> combatAttributes, 
            IList<AbilityName> abilities) : base(name, displayName, description, statistics, originalFaction, combatAttributes, abilities)
        {
        }
    }
}
