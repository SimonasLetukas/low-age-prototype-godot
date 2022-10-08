﻿using System.Collections.Generic;
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
            Vector2<int>? size = null,
            Vector2<int>? centerPoint = null,
            bool? destructible = null,
            bool? startingStructure = null,
            Area? walkableArea = null) : base(name, displayName, description, statistics, originalFaction, combatAttributes, abilities)
        {
            Size = size ?? new Vector2<int>(1, 1);
            CenterPoint = centerPoint ?? (Size == new Vector2<int>(1, 1)
                ? new Vector2<int>(0, 0)
                : new Vector2<int>(Size.X / 2, Size.Y / 2));
            Destructible = destructible ?? true;
            StartingStructure = startingStructure ?? false;
            WalkableArea = walkableArea ?? new Area(new Vector2<int>(0, 0));
        }

        /// <summary>
        /// Width and height of the <see cref="Structure"/>. Starts from 1.
        /// </summary>
        public Vector2<int> Size { get; }
        
        /// <summary>
        /// Coordinates of the center point, used for rotating and placing purposes. Starts from 0. 
        /// </summary>
        public Vector2<int> CenterPoint { get; }

        /// <summary>
        /// If true, this <see cref="Structure"/> can be targeted and attacked, shows health and other statistics. 
        /// </summary>
        public bool Destructible { get; }
        
        /// <summary>
        /// If true, this <see cref="Structure"/> is placed on the starting map location for the
        /// <see cref="Actor.OriginalFaction"/>. It is also rotated to face as many enemies as possible. 
        /// </summary>
        public bool StartingStructure { get; }
        
        /// <summary>
        /// Specifies which part of the <see cref="Structure"/>'s <see cref="Area"/> is walkable at ground level. This
        /// is usually used to reserve space in which units can be created but other buildings cannot be placed. 
        /// </summary>
        public Area WalkableArea { get; }
    }
}
