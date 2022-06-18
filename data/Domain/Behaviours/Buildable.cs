using System.Collections.Generic;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// Can be added to a buildable <see cref="Actor"/> so that further building options could be configured. This
    /// behaviour is automatically removed after building process is finished.
    /// </summary>
    public class Buildable : Behaviour
    {
        public Buildable(
            BehaviourName name,
            string displayName, 
            string description,
            IList<Validator>? placementValidators = null,
            int? maximumHelpers = null) : base(name, $"{nameof(Behaviour)}.{nameof(Counter)}", displayName, description, EndsAt.Death)
        {
            PlacementValidators = placementValidators ?? new List<Validator>();
            MaximumHelpers = maximumHelpers ?? -1;
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
    }
}