using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Entities.Tiles;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Entities.Features
{
    public class Feature : Entity
    { 
        public Feature(
            FeatureName name,
            string displayName, 
            string description,
            EffectName? onCollisionEffect = null,
            IList<Flag>? collisionFilters = null,
            EffectName? periodicEffect = null,
            int? size = null,
            bool? canBeAttacked = null,
            bool? occupiesSpace = null,
            bool? statsCopiedFromSource = null,
            bool? alliesCanStack = null,
            bool? onlyOneCanExist = null) : base(name, displayName, description)
        {
            OnCollisionEffect = onCollisionEffect;
            CollisionFilters = collisionFilters ?? new List<Flag>();
            PeriodicEffect = periodicEffect;
            Size = size ?? 1;
            CanBeAttacked = canBeAttacked ?? false;
            OccupiesSpace = occupiesSpace ?? false;
            StatsCopiedFromSource = statsCopiedFromSource ?? false;
            AlliesCanStack = alliesCanStack ?? false;
            OnlyOneCanExist = onlyOneCanExist ?? false;
        }

        public EffectName? OnCollisionEffect { get; }
        public IList<Flag> CollisionFilters { get; }
        
        /// <summary>
        /// Executes effect as often as possible
        /// </summary>
        public EffectName? PeriodicEffect { get; }
        
        public int Size { get; }
        
        /// <summary>
        /// If true, this <see cref="Feature"/> can be attacked. <see cref="Stats.Health"/> can be set indirectly
        /// through <see cref="Buff"/>s, or by using <see cref="StatsCopiedFromSource"/>, or <see cref="Tether"/>ing to
        /// another unit and having <see cref="Tether.SharedDamage"/> set to true (this list might change as data
        /// evolves).
        /// </summary>
        public bool CanBeAttacked { get; }
        
        /// <summary>
        /// If true, this <see cref="Feature"/> imposes the same pathfinding rules as a <see cref="Unit"/>: no other
        /// <see cref="Actor"/> can end up on top of this <see cref="Feature"/>, friendly <see cref="Unit"/>s can pass
        /// through but enemy <see cref="Unit"/>s cannot. 
        /// </summary>
        public bool OccupiesSpace { get; }
        
        /// <summary>
        /// If true, this <see cref="Feature"/> receives a shadow copy of the <see cref="Stats"/> that the
        /// <see cref="Location.Source"/> had at the point of <see cref="CreateEntity"/>. This is useful when
        /// further <see cref="Effect"/>s that this <see cref="Feature"/> executes should be based on
        /// <see cref="Location.Source"/>'s <see cref="Stats"/>. This also includes the <see cref="Unit.Size"/>.
        /// </summary>
        public bool StatsCopiedFromSource { get; }
        
        /// <summary>
        /// If true, multiple <see cref="Feature"/>s from the same team can co-exist on the same <see cref="Tile"/>.
        /// By default, allies cannot stack <see cref="Feature"/>s, but <see cref="Feature"/>s from different teams
        /// can co-exist.
        /// </summary>
        public bool AlliesCanStack { get; }
        
        /// <summary>
        /// If true, only one <see cref="Feature"/> can exist on the same <see cref="Tile"/> across any number of
        /// teams and <see cref="AlliesCanStack"/> input is overriden. 
        /// </summary>
        public bool OnlyOneCanExist { get; }
    }
}
