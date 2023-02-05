using System;
using System.Collections.Generic;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// Used to specify <see cref="BlockedAreas"/> for certain filtered <see cref="Entity"/>s. Relevant for calculating
    /// pathfinding.
    /// </summary>
    public class MovementBlock : Behaviour
    {
        public MovementBlock(
            BehaviourName name,
            string displayName, 
            string description,
            IList<Area> blockedAreas,
            IList<Flag> filterFlags) 
            : base(
                name, 
                $"{nameof(Behaviour)}.{nameof(MovementBlock)}", 
                displayName, 
                description, 
                EndsAt.Death,
                Alignment.Neutral)
        {
            BlockedAreas = blockedAreas.Count == 0
                ? throw new ArgumentOutOfRangeException(nameof(blockedAreas), 
                    $"Must contain at least one {nameof(blockedAreas)} element")
                : blockedAreas;
            FilterFlags = filterFlags;
        }
        
        /// <summary>
        /// Specify the <see cref="Area"/>s which will block movement.
        /// </summary>
        public IList<Area> BlockedAreas { get; }
        
        /// <summary>
        /// <see cref="Flag.Filter"/> the <see cref="Entity"/>s which should not move through the
        /// <see cref="BlockedAreas"/>.
        /// </summary>
        public IList<Flag> FilterFlags { get; }
    }
}