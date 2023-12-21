using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Common;
using low_age_data.Domain.Factions;

namespace low_age_data.Domain.Entities.Actors
{
    public abstract class Actor : Entity, IDisplayable
    {
        protected Actor(
            EntityId id, 
            string displayName, 
            string description,
            string sprite,
            IList<Stat> statistics,
            FactionId originalFaction,
            IList<ActorAttribute> actorAttributes,
            IList<AbilityId> abilities) : base(id, displayName, description)
        {
            Statistics = statistics;
            OriginalFaction = originalFaction;
            ActorAttributes = actorAttributes;
            Abilities = abilities;
            Sprite = sprite;
        }

        public IList<Stat> Statistics { get; }
        public FactionId OriginalFaction { get; }
        public IList<ActorAttribute> ActorAttributes { get; }
        public IList<AbilityId> Abilities { get; }
        public string? Sprite { get; } 
    }
}
