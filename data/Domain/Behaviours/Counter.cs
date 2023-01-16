using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// Used for counting and starting effects depending on count amount.
    /// </summary>
    public class Counter : Behaviour
    {
        public Counter(
            BehaviourName name,
            string displayName, 
            string description,
            int maxAmount,
            int triggerAmount,
            IList<EffectName> triggeredEffects, 
            bool? selfRemoveAfterAmountReached = null,
            bool? triggerAtMultiples = null,
            EndsAt? endsAt = null,
            bool? separateDurations = null,
            bool? canResetDuration = null) 
            : base(
                name, 
                $"{nameof(Behaviour)}.{nameof(Counter)}", 
                displayName, 
                description, 
                endsAt ?? EndsAt.Death)
        {
            MaxAmount = maxAmount;
            TriggerAmount = triggerAmount;
            TriggeredEffects = triggeredEffects;
            SelfRemoveAfterAmountReached = selfRemoveAfterAmountReached ?? false;
            TriggerAtMultiples = triggerAtMultiples ?? false;
            SeparateDurations = separateDurations ?? false;
            CanResetDuration = canResetDuration ?? false;
        }
        
        /// <summary>
        /// Counter counts up to (and including) this amount. 
        /// </summary>
        public int MaxAmount { get; }
        
        /// <summary>
        /// Amount at which the <see cref="TriggeredEffects"/> are executed.
        /// </summary>
        public int TriggerAmount { get; }
        
        /// <summary>
        /// If true, any multiples of <see cref="TriggerAmount"/> will also execute the <see cref="TriggeredEffects"/>.
        /// </summary>
        public bool TriggerAtMultiples { get; }
        
        /// <summary>
        /// If true, this behaviour is removed once <see cref="MaxAmount"/> is reached and <see cref="TriggeredEffects"/>
        /// are executed.
        /// </summary>
        public bool SelfRemoveAfterAmountReached { get; }
        
        public IList<EffectName> TriggeredEffects { get; }

        /// <summary>
        /// If true, each count has its own duration (controlled by <see cref="Behaviour.EndsAt"/>); if false,
        /// all counts share the same duration.
        /// </summary>
        public bool SeparateDurations { get; }
        
        /// <summary>
        /// If true, each new count resets the duration. Only valid if <see cref="SeparateDurations"/> is false. 
        /// </summary>
        public bool CanResetDuration { get; }
    }
}