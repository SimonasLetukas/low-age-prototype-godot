using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Factions;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Entities.Actors
{
    public abstract class Actor : Entity
    {
        protected Actor(
            EntityName name, 
            string displayName, 
            string description,
            IList<Stat> statistics,
            FactionName originalFaction,
            IList<CombatAttribute> combatAttributes,
            IList<AbilityName> abilities) : base(name, displayName, description)
        {
            Statistics = statistics;
            OriginalFaction = originalFaction;
            CombatAttributes = combatAttributes;
            Abilities = abilities;
        }

        public IList<Stat> Statistics { get; }
        public FactionName OriginalFaction { get; }
        public IList<CombatAttribute> CombatAttributes { get; }
        public IList<AbilityName> Abilities { get; }
    }
}
