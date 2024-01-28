using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Common;
using low_age_data.Domain.Factions;

namespace low_age_data.Domain.Entities.Actors.Units
{
    public class Unit : Actor
    {
        public Unit(
            UnitId id, 
            string displayName, 
            string description, 
            string sprite,
            Vector2<int> centerOffset,
            IList<Stat> statistics, 
            FactionId originalFaction, 
            IList<ActorAttribute> actorAttributes, 
            IList<AbilityId> abilities,
            int? size = null) : base(
            id, 
            displayName, 
            description, 
            sprite, 
            centerOffset,
            statistics, 
            originalFaction, 
            actorAttributes, 
            abilities)
        {
            Size = size ?? 1;
        }

        public int Size { get; }
    }
}
