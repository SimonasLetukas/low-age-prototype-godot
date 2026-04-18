using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Common.Modifications;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Logic;
using LowAgeData.Domain.Resources;

namespace LowAgeData.Domain.Behaviours
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
            bool? instantUpdate = null,
            EndsAt? endsAt = null,
            Alignment? alignment = null,
            IList<Payment>? cost = null,
            bool? overflowPayment = null,
            bool? waitForAvailableStorage = null,
            IList<Trigger>? triggers = null,
            bool? removeOnConditionsMet = null,
            IList<EffectId>? conditionalEffects = null) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
                endsAt ?? EndsAt.Death,
                alignment ?? Alignment.Positive,
                triggers: triggers,
                removeOnConditionsMet: removeOnConditionsMet,
                conditionalEffects: conditionalEffects)
        {
            Resources = resources;
            DiminishingReturn = diminishingReturn ?? 0;
            InstantUpdate = instantUpdate ?? false;
            Cost = cost ?? new List<Payment>();
            OverflowPayment = overflowPayment ?? false;
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
        /// <para>
        /// If true, when this behaviour is added or removed, the associated <see cref="Resources"/> are instantly
        /// updated (i.e. all other <see cref="Income"/>s are recalculated for each registered <see cref="Resource"/>
        /// in <see cref="Resources"/> -- each <see cref="Income"/> is checked for <see cref="Cost"/> again, so the
        /// updated values might be different, because the deduction needed for <see cref="Cost"/> would not happen
        /// here).
        /// </para>
        /// <para>
        /// Otherwise, the <see cref="Resources"/> are gained normally (start of planning phase).
        /// </para>
        /// <para>
        /// Default = false.
        /// </para>
        /// </summary>
        public bool InstantUpdate { get; }
        
        /// <summary>
        /// Deducted before updating the income at the start of each planning phase. If the <see cref="Cost"/> is
        /// fulfilled then <see cref="Resources"/> are gained as <see cref="Income"/>. Otherwise, <see cref="Cost"/>
        /// accumulates until it is fulfilled.
        /// </summary>
        public IList<Payment> Cost { get; }
        
        /// <summary>
        /// <para>
        /// If true, any non-consumable income that is paid to the remaining <see cref="Cost"/> that would go
        /// over the limit is instead retained for the next <see cref="Cost"/> calculation. E.g. income is 5,
        /// current cost paid is 3/7, after payment current cost is 1/7 instead of 0/7.
        /// </para>
        /// <para>
        /// Default = false.
        /// </para>
        /// </summary>
        public bool OverflowPayment { get; }
        
        /// <summary>
        /// If true, <see cref="Resources"/> are not gained and <see cref="Cost"/> is paused if there is not enough
        /// storage space for a resource. False by default. 
        /// </summary>
        public bool WaitForAvailableStorage { get; }
    }
}