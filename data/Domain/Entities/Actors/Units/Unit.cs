﻿using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Factions;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Entities.Actors.Units
{
    public class Unit : Actor
    {
        public Unit(
            UnitId id, 
            string displayName, 
            string description, 
            string sprite,
            IList<Stat> statistics, 
            FactionId originalFaction, 
            IList<ActorAttribute> combatAttributes, 
            IList<AbilityId> abilities,
            int? size = null) : base(id, displayName, description, sprite, statistics, originalFaction, combatAttributes, abilities)
        {
            Size = size ?? 1;
        }

        public int Size { get; }
    }
}
