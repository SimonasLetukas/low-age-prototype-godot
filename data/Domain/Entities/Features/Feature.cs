using System.Collections.Generic;
using low_age_data.Domain.Effects;
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
            // TODO flag for continuous collision every turn?
            IList<Flag>? collisionFilters = null,
            int? size = null,
            bool? statsCopiedFromSource = null) : base(name, displayName, description)
        {
            OnCollisionEffect = onCollisionEffect;
            CollisionFilters = collisionFilters ?? new List<Flag>();
            Size = size ?? 1;
            StatsCopiedFromSource = statsCopiedFromSource ?? false;
        }

        public EffectName? OnCollisionEffect { get; }
        public IList<Flag> CollisionFilters { get; }
        public int Size { get; }
        
        /// <summary>
        /// If true, this <see cref="Feature"/> receives a shadow copy of the <see cref="Stats"/> that the
        /// <see cref="Location.Source"/> had at the point of <see cref="CreateEntity"/>. This is useful when
        /// further <see cref="Effect"/>s that this <see cref="Feature"/> executes should be based on
        /// <see cref="Location.Source"/>'s <see cref="Stats"/>. 
        /// </summary>
        public bool StatsCopiedFromSource { get; }
    }
}
