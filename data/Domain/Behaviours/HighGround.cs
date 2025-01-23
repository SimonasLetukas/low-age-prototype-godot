using System;
using System.Collections.Generic;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Entities.Actors;
using LowAgeData.Domain.Entities.Actors.Units;

namespace LowAgeData.Domain.Behaviours
{
    /// <summary>
    /// Can be added to an <see cref="Actor"/> to make part of its size host high ground for other <see cref="Unit"/>s
    /// to navigate through. Pathfinding wise, the high ground tiles only connect to other adjacent high ground tiles
    /// or tiles marked as <see cref="Ascendable"/>.
    /// </summary>
    public class HighGround : Behaviour
    {
        public HighGround(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            IList<HighGroundArea> highGroundAreas,
            IList<EffectId>? onCollisionEffects = null) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
                EndsAt.Death,
                Alignment.Neutral)
        {
            HighGroundAreas = highGroundAreas.Count == 0
                ? throw new ArgumentOutOfRangeException(nameof(highGroundAreas), 
                    $"Must contain at least one {nameof(highGroundAreas)} element")
                : highGroundAreas;
            OnCollisionEffects = onCollisionEffects ?? new List<EffectId>
            {
                EffectId.Shared.HighGroundSearch
            };
        }
        
        /// <summary>
        /// Specifies the areas inside the <see cref="Actor"/>'s size to become high ground. Multiple
        /// <see cref="HighGroundAreas"/> can intersect on the same tile.
        /// </summary>
        public IList<HighGroundArea> HighGroundAreas { get; }
        
        /// <summary>
        /// By default, all units gain +1 Ranged Attack Distance when on high ground, but
        /// <see cref="OnCollisionEffects"/> could also be used to add different effects.
        /// </summary>
        public IList<EffectId> OnCollisionEffects { get; }
    }
}