using System.Collections.Generic;
using low_age_data.Domain.Effects;
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
            bool? statsCopiedFromSource = null,
            bool? alliesCanStack = null,
            bool? onlyOneCanExist = null) : base(name, displayName, description)
        {
            OnCollisionEffect = onCollisionEffect;
            CollisionFilters = collisionFilters ?? new List<Flag>();
            Size = size ?? 1;
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
        /// If true, this <see cref="Feature"/> receives a shadow copy of the <see cref="Stats"/> that the
        /// <see cref="Location.Source"/> had at the point of <see cref="CreateEntity"/>. This is useful when
        /// further <see cref="Effect"/>s that this <see cref="Feature"/> executes should be based on
        /// <see cref="Location.Source"/>'s <see cref="Stats"/>. 
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
