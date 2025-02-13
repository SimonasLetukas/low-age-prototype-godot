using System.Collections.Generic;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Factions;
using LowAgeCommon;

namespace LowAgeData.Domain.Entities.Actors.Units
{
    public class Unit : Actor
    {
        public Unit(
            UnitId id, 
            string displayName, 
            string description, 
            string sprite,
            Vector2Int centerOffset,
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
