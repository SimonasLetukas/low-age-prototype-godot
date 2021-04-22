using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Entities.Features
{
    public class Feature : Entity
    {
        public Feature(
            FeatureName name, 
            string displayName, 
            string description,
            EffectName onCollisionEffect,
            IList<Flag>? collisionFilters = null,
            int? size = null) : base(name, displayName, description)
        {
            OnCollisionEffect = onCollisionEffect;
            CollisionFilters = collisionFilters ?? new List<Flag>();
            Size = size ?? 1;
        }

        public EffectName OnCollisionEffect { get; }
        public IList<Flag> CollisionFilters { get; }
        public int Size { get; }
    }
}
