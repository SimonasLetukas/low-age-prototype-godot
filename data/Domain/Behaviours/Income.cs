using System.Collections.Generic;
using low_age_data.Domain.Entities;
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
            EndsAt? endsAt = null) 
            : base(
                name, 
                $"{nameof(Behaviour)}.{nameof(Income)}", 
                displayName, 
                description, 
                endsAt ?? EndsAt.Death)
        {
            Resources = resources;
            DiminishingReturn = diminishingReturn ?? 0;
        }

        /// <summary>
        /// Types and amounts of resources to get at the start of each planning phase.
        /// </summary>
        public IList<ResourceModification> Resources { get; }
        
        /// <summary>
        /// Value which is deducted from the <see cref="Resources"/> <see cref="Modification.Amount"/> for each
        /// <see cref="Income"/> of the same <see cref="BehaviourName"/>. Final <see cref="Modification.Amount"/>
        /// cannot be lower than 1.
        /// </summary>
        public int DiminishingReturn { get; }
    }
}