using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Logic;
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
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            IList<ResourceModification> resources,
            int? diminishingReturn = null,
            EndsAt? endsAt = null,
            IList<Payment>? cost = null,
            bool? waitForAvailableStorage = null,
            IList<Trigger>? triggers = null,
            bool? removeOnConditionsMet = null,
            IList<EffectId>? conditionalEffects = null) 
            : base(
                id, 
                $"{nameof(Behaviour)}.{nameof(Income)}", 
                displayName, 
                description, 
                sprite,
                endsAt ?? EndsAt.Death,
                Alignment.Positive,
                triggers: triggers,
                removeOnConditionsMet: removeOnConditionsMet,
                conditionalEffects: conditionalEffects)
        {
            Resources = resources;
            DiminishingReturn = diminishingReturn ?? 0;
            Cost = cost ?? new List<Payment>();
            WaitForAvailableStorage = waitForAvailableStorage ?? false;
        }

        /// <summary>
        /// Types and amounts of resources to get at the start of each planning phase.
        /// </summary>
        public IList<ResourceModification> Resources { get; }
        
        /// <summary>
        /// Value which is deducted from the <see cref="Resources"/> <see cref="Modification.Amount"/> for each
        /// subsequent <see cref="Income"/> of the same <see cref="BehaviourId"/>. Final
        /// <see cref="Modification.Amount"/> cannot be lower than 1.
        /// </summary>
        public int DiminishingReturn { get; }
        
        /// <summary>
        /// Deducted just before the start of each planning phase. If the <see cref="Cost"/> is fulfilled then
        /// <see cref="Resources"/> are gained as <see cref="Income"/>. Otherwise, <see cref="Cost"/> accumulates
        /// until it is fulfilled.
        /// </summary>
        public IList<Payment> Cost { get; }
        
        /// <summary>
        /// If true, <see cref="Resources"/> are not gained and <see cref="Cost"/> is paused if there is not enough
        /// storage space for a resource. False by default. 
        /// </summary>
        public bool WaitForAvailableStorage { get; }
    }
}