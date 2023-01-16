using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;
using low_age_data.Domain.Shared.Modifications;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// Added to <see cref="Entity"/> to provide continuous income. 
    /// </summary>
    public class Income : Behaviour
    {
        public Income(
            BehaviourName name,
            string displayName, 
            string description,
            IList<ResourceModification> resources,
            int? diminishingReturn = null,
            EndsAt? endsAt = null,
            IList<Cost>? cost = null,
            bool? waitForAvailableStorage = null) 
            : base(
                name, 
                $"{nameof(Behaviour)}.{nameof(Income)}", 
                displayName, 
                description, 
                endsAt ?? EndsAt.Death)
        {
            Resources = resources;
            DiminishingReturn = diminishingReturn ?? 0;
            Cost = cost ?? new List<Cost>();
            WaitForAvailableStorage = waitForAvailableStorage ?? false;
        }

        /// <summary>
        /// Types and amounts of resources to get at the start of each planning phase.
        /// </summary>
        public IList<ResourceModification> Resources { get; }
        
        /// <summary>
        /// Value which is deducted from the <see cref="Resources"/> <see cref="Modification.Amount"/> for each
        /// subsequent <see cref="Income"/> of the same <see cref="BehaviourName"/>. Final
        /// <see cref="Modification.Amount"/> cannot be lower than 1.
        /// </summary>
        public int DiminishingReturn { get; }
        
        /// <summary>
        /// Deducted just before the start of each planning phase. If the <see cref="Cost"/> is fulfilled then
        /// <see cref="Resources"/> are gained as <see cref="Income"/>. Otherwise, <see cref="Cost"/> accumulates
        /// until it is fulfilled.
        /// </summary>
        public IList<Cost> Cost { get; }
        
        /// <summary>
        /// If true, <see cref="Resources"/> are not gained and <see cref="Cost"/> is paused if there is not enough
        /// storage space for a resource. False by default. 
        /// </summary>
        public bool WaitForAvailableStorage { get; }
    }
}