using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Common;
using low_age_data.Domain.Factions;
using low_age_prototype_common;

namespace low_age_data.Domain.Entities.Actors.Structures
{
    public class Structure : Actor, IRotatable
    {
        public Structure(
            StructureId id,
            string displayName,
            string description,
            string sprite,
            Vector2<int> centerOffset,
            IList<Stat> statistics,
            FactionId originalFaction,
            IList<ActorAttribute> actorAttributes,
            IList<AbilityId> abilities,
            string? backSideSprite = null,
            Vector2<int>? backSideCenterOffset = null,
            Vector2<int>? size = null,
            Vector2<int>? centerPoint = null,
            bool? destructible = null,
            IList<Area>? walkableAreas = null,
            string? flattenedSprite = null,
            Vector2<int>? flattenedCenterOffset = null) 
            : base(
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
            Size = size ?? new Vector2<int>(1, 1);
            CenterPoint = centerPoint ?? (Size.Equals(new Vector2<int>(1, 1))
                ? new Vector2<int>(0, 0)
                : new Vector2<int>(Size.X / 2, Size.Y / 2));
            Destructible = destructible ?? true;
            WalkableAreas = walkableAreas ?? new List<Area>
            {
                new Area(new Vector2<int>(0, 0))
            };
            BackSideSprite = backSideSprite ?? sprite;
            BackSideCenterOffset = backSideCenterOffset ?? centerOffset;
            FlattenedSprite = flattenedSprite;
            FlattenedCenterOffset = flattenedCenterOffset;
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
        /// Specifies which part of the <see cref="Structure"/>'s <see cref="Area"/> is walkable at ground level. This
        /// is usually used to reserve space in which units can be created but other buildings cannot be placed. 
        /// </summary>
        public IList<Area> WalkableAreas { get; }
        
        public string? BackSideSprite { get; }
        
        public Vector2<int> BackSideCenterOffset { get; }
        
        /// <summary>
        /// Path to the sprite to be used for showing the <see cref="Structure"/> in a flattened state.
        /// </summary>
        public string? FlattenedSprite { get; }
        
        public Vector2<int>? FlattenedCenterOffset { get; }
    }
}