using System.Collections.Generic;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Factions;
using low_age_prototype_common;

namespace LowAgeData.Domain.Entities.Actors
{
    public abstract class Actor : Entity
    {
        protected Actor(
            EntityId id, 
            string displayName, 
            string description,
            string sprite,
            Vector2<int> centerOffset,
            IList<Stat> statistics,
            FactionId originalFaction,
            IList<ActorAttribute> actorAttributes,
            IList<AbilityId> abilities) : base(id, displayName, description, sprite, centerOffset)
        {
            Statistics = statistics;
            OriginalFaction = originalFaction;
            ActorAttributes = actorAttributes;
            Abilities = abilities;
        }

        public IList<Stat> Statistics { get; }
        public FactionId OriginalFaction { get; }
        public IList<ActorAttribute> ActorAttributes { get; }
        public IList<AbilityId> Abilities { get; }
    }
}
