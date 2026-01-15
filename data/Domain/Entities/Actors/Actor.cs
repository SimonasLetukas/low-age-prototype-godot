using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Factions;
using LowAgeCommon;

namespace LowAgeData.Domain.Entities.Actors
{
    public abstract class Actor : Entity
    {
        protected Actor(
            EntityId id, 
            string displayName, 
            string description,
            string sprite,
            Vector2Int centerOffset,
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
        
        /// <summary>
        /// List of <see cref="Ability"/>s this <see cref="Actor"/> is able to execute. Every
        /// <see cref="Ability"/> must be unique, i.e. multiple abilities with the same <see cref="AbilityId"/>
        /// will not be preserved.
        /// </summary>
        public IList<AbilityId> Abilities { get; }
    }
}
