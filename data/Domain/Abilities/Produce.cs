﻿using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Entities.Actors.Structures;

namespace LowAgeData.Domain.Abilities
{
    /// <summary>
    /// Used for making a new <see cref="Entity"/> out of an existing <see cref="Entity"/>.
    ///
    /// Note: no adequate UX could be designed for this ability, so for now <see cref="Build"/> have taken over. To be
    /// revisited in the future if the need arises to be able to have queues and work on entities safely inside of other
    /// entities.
    /// </summary>
    public class Produce : Ability
    {
        public Produce(
            AbilityId id,
            string displayName,
            string description,
            string sprite,
            IList<Selection<EntityId>> selection,
            bool? canPlaceInWalkableAreaOnly = null,
            bool? hasQueue = null,
            bool? producedInstantly = null)
            : base(
                id,
                TurnPhase.Planning,
                new List<ResearchId>(),
                true,
                displayName,
                description,
                sprite)
        {
            Selection = selection;
            CanPlaceInWalkableAreaOnly = canPlaceInWalkableAreaOnly ?? false;
            HasQueue = hasQueue ?? false;
            ProducedInstantly = producedInstantly ?? false;
        }

        public IList<Selection<EntityId>> Selection { get; }

        /// <summary>
        /// If true, placement for a new <see cref="Entity"/> to <see cref="Produce"/> in is only allowed inside
        /// <see cref="Structure"/>'s <see cref="Structure.WalkableAreas"/>.
        /// </summary>
        public bool CanPlaceInWalkableAreaOnly { get; }

        /// <summary>
        /// If true, producing new entities puts them in a queue.
        /// </summary>
        public bool HasQueue { get; }

        /// <summary>
        /// If true, <see cref="Entity"/> is <see cref="Produce"/>d instantly after placing it and paying the cost.
        /// </summary>
        public bool ProducedInstantly { get; }
    }
}