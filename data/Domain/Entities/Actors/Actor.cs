using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Factions;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Entities.Actors
{
    public abstract class Actor : Entity
    {
        protected Actor(
            EntityId id, 
            string displayName, 
            string description,
            IList<Stat> statistics,
            FactionId originalFaction,
            IList<CombatAttribute> combatAttributes,
            IList<AbilityId> abilities) : base(id, displayName, description)
        {
            Statistics = statistics;
            OriginalFaction = originalFaction;
            CombatAttributes = combatAttributes;
            Abilities = abilities;
        }

        public IList<Stat> Statistics { get; }
        public FactionId OriginalFaction { get; }
        public IList<CombatAttribute> CombatAttributes { get; }
        public IList<AbilityId> Abilities { get; }
    }
}
