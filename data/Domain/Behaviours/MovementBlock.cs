using System;
using System.Collections.Generic;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Durations;
using low_age_data.Domain.Common.Filters;
using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// Used to specify <see cref="BlockedAreas"/> for certain filtered <see cref="Entity"/>s. Relevant for calculating
    /// pathfinding.
    /// </summary>
    public class MovementBlock : Behaviour
    {
        public MovementBlock(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            IList<Area> blockedAreas,
            IList<IFilterItem> filters) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
                EndsAt.Death,
                Alignment.Neutral)
        {
            BlockedAreas = blockedAreas.Count == 0
                ? throw new ArgumentOutOfRangeException(nameof(blockedAreas), 
                    $"Must contain at least one {nameof(blockedAreas)} element")
                : blockedAreas;
            Filters = filters;
        }
        
        /// <summary>
        /// Specify the <see cref="Area"/>s which will block movement.
        /// </summary>
        public IList<Area> BlockedAreas { get; }
        
        /// <summary>
        /// Filter the <see cref="Entity"/>s which should not move through the
        /// <see cref="BlockedAreas"/>.
        /// </summary>
        public IList<IFilterItem> Filters { get; }
    }
}