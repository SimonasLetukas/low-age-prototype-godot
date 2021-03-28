using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Entities.Actors.Structures
{
    public class Structure : Actor
    {
        public Structure(
            StructureName name, 
            string displayName, 
            string description, 
            IList<Stat> statistics, 
            Factions originalFaction, 
            IList<CombatAttributes> combatAttributes, 
            IList<AbilityName> abilities,
            int? width = null,
            int? height = null) : base(name, displayName, description, statistics, originalFaction, combatAttributes, abilities)
        {
            Width = width ?? 1;
            Height = height ?? 1;
        }

        public int Width { get; }
        public int Height { get; }
    }
}
