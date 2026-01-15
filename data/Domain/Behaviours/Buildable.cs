using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Entities.Actors;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Behaviours
{
    /// <summary>
    /// Can be added to a buildable <see cref="Entity"/> so that further building options could be configured. This
    /// behaviour is automatically removed after building process is finished. Until then it tracks the progress of
    /// how much of the required cost is paid.
    /// 
    /// <see cref="Build"/> <see cref="Ability"/> also checks for preconditions configured in this
    /// <see cref="Behaviour"/> when selecting the <see cref="Buildable"/> <see cref="Actor"/> (but before placing it),
    /// such as: <see cref="PlacementValidators"/> and <see cref="CanBeDragged"/>.
    /// </summary>
    public class Buildable : Behaviour
    {
        public Buildable(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            IList<Validator>? placementValidators = null,
            int? maximumHelpers = null,
            bool? canBeDragged = null) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
                EndsAt.Death,
                Alignment.Neutral)
        {
            PlacementValidators = placementValidators ?? new List<Validator>();
            MaximumHelpers = maximumHelpers ?? -1;
            CanBeDragged = canBeDragged ?? false;
        }

        /// <summary>
        /// <b>All</b> <see cref="PlacementValidators"/> need to succeed for the placement of the buildable
        /// <see cref="Actor"/> to be valid.
        /// </summary>
        public IList<Validator> PlacementValidators { get; }
        
        /// <summary>
        /// Amount after which no more helpers are allowed to speed up the building process. Value of -1 means there
        /// is no maximum.
        /// </summary>
        public int MaximumHelpers { get; }
        
        /// <summary>
        /// If true, holding down the button to place the <see cref="Buildable"/> <see cref="Actor"/> will place
        /// multiple entities of the same type in a line over the dragged distance. False by default.
        /// </summary>
        public bool CanBeDragged { get; }
    }
}