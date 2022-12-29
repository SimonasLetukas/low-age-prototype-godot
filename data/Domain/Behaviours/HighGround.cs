using System;
using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// Can be added to an <see cref="Actor"/> to make part of its size host high ground for other <see cref="Unit"/>s
    /// to navigate through. Pathfinding wise, the high ground tiles only connect to other adjacent high ground tiles
    /// or tiles marked as <see cref="Ascendable"/>.
    /// </summary>
    public class HighGround : Behaviour
    {
        public HighGround(
            BehaviourName name,
            string displayName, 
            string description,
            IList<Area> highGroundAreas,
            IList<EffectName>? onCollisionEffects = null) : base(name, $"{nameof(Behaviour)}.{nameof(HighGround)}", displayName, description, EndsAt.Death)
        {
            HighGroundAreas = highGroundAreas.Count == 0
                ? throw new ArgumentOutOfRangeException(nameof(highGroundAreas), 
                    $"Must contain at least one {nameof(highGroundAreas)} element")
                : highGroundAreas;
            OnCollisionEffects = onCollisionEffects ?? new List<EffectName>
            {
                EffectName.Shared.HighGroundSearch
            };
        }
        
        /// <summary>
        /// Specifies the areas inside the <see cref="Actor"/>'s size to become high ground. Multiple
        /// <see cref="HighGroundAreas"/> can intersect on the same tile.
        /// </summary>
        public IList<Area> HighGroundAreas { get; }
        
        /// <summary>
        /// By default, all units gain +1 Ranged Attack Distance when on high ground, but
        /// <see cref="OnCollisionEffects"/> could also be used to add different effects.
        /// </summary>
        public IList<EffectName> OnCollisionEffects { get; }
    }
}