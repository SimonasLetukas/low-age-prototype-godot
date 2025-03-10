﻿using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Entities.Actors;
using LowAgeData.Domain.Entities.Actors.Units;
using LowAgeData.Domain.Tiles;
using LowAgeCommon;

namespace LowAgeData.Domain.Entities.Doodads
{
    public class Doodad : Entity
    { 
        public Doodad(
            DoodadId id,
            string displayName, 
            string description, 
            string sprite, 
            Vector2Int centerOffset,
            EffectId? onCollisionEffect = null,
            IList<IFilterItem>? collisionFilters = null,
            EffectId? periodicEffect = null,
            int? size = null,
            bool? canBeAttacked = null,
            bool? occupiesSpace = null,
            bool? statsCopiedFromSource = null,
            bool? alliesCanStack = null,
            bool? onlyOneCanExist = null) : base(id, displayName, description, sprite, centerOffset)
        {
            OnCollisionEffect = onCollisionEffect;
            CollisionFilters = collisionFilters ?? new List<IFilterItem>();
            PeriodicEffect = periodicEffect;
            Size = size ?? 1;
            CanBeAttacked = canBeAttacked ?? false;
            OccupiesSpace = occupiesSpace ?? false;
            StatsCopiedFromSource = statsCopiedFromSource ?? false;
            AlliesCanStack = alliesCanStack ?? false;
            OnlyOneCanExist = onlyOneCanExist ?? false;
        }

        public EffectId? OnCollisionEffect { get; }
        public IList<IFilterItem> CollisionFilters { get; }
        
        /// <summary>
        /// Executes effect as often as possible
        /// </summary>
        public EffectId? PeriodicEffect { get; }
        
        public int Size { get; }
        
        /// <summary>
        /// If true, this <see cref="Doodad"/> can be attacked. <see cref="StatType.Health"/> can be set indirectly
        /// through <see cref="Buff"/>s, or by using <see cref="StatsCopiedFromSource"/>, or <see cref="Tether"/>ing to
        /// another unit and having <see cref="Tether.SharedDamage"/> set to true (this list might change as data
        /// evolves).
        /// </summary>
        public bool CanBeAttacked { get; }
        
        /// <summary>
        /// If true, this <see cref="Doodad"/> imposes the same pathfinding rules as a <see cref="Unit"/>: no other
        /// <see cref="Actor"/> can end up on top of this <see cref="Doodad"/>, friendly <see cref="Unit"/>s can pass
        /// through but enemy <see cref="Unit"/>s cannot. 
        /// </summary>
        public bool OccupiesSpace { get; }
        
        /// <summary>
        /// If true, this <see cref="Doodad"/> receives a shadow copy of the <see cref="StatType"/> that the
        /// <see cref="Location.Source"/> had at the point of <see cref="CreateEntity"/>. This is useful when
        /// further <see cref="Effect"/>s that this <see cref="Doodad"/> executes should be based on
        /// <see cref="Location.Source"/>'s <see cref="StatType"/>. This also includes the <see cref="Unit.Size"/>.
        /// </summary>
        public bool StatsCopiedFromSource { get; }
        
        /// <summary>
        /// If true, multiple <see cref="Doodad"/>s from the same team can co-exist on the same <see cref="Tile"/>.
        /// By default, allies cannot stack <see cref="Doodad"/>s, but <see cref="Doodad"/>s from different teams
        /// can co-exist.
        /// </summary>
        public bool AlliesCanStack { get; }
        
        /// <summary>
        /// If true, only one <see cref="Doodad"/> can exist on the same <see cref="Tile"/> across any number of
        /// teams and <see cref="AlliesCanStack"/> input is overriden. 
        /// </summary>
        public bool OnlyOneCanExist { get; }
    }
}
