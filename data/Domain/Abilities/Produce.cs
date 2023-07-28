using System.Collections.Generic;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Abilities
{
    public class Produce : Ability
    {
        /// <summary>
        /// Used for making a new <see cref="Entity"/> out of an existing <see cref="Entity"/>.
        /// </summary>
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
                $"{nameof(Ability)}.{nameof(Produce)}",
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
        /// <see cref="Structure"/>'s <see cref="Structure.WalkableArea"/>.
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