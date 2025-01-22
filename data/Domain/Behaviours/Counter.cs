using System.Collections.Generic;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Effects;

namespace LowAgeData.Domain.Behaviours
{
    /// <summary>
    /// Used for counting and starting effects depending on count amount.
    /// </summary>
    public class Counter : Behaviour
    {
        public Counter(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            int maxAmount,
            int triggerAmount,
            IList<EffectId> triggeredEffects, 
            bool? selfRemoveAfterAmountReached = null,
            bool? triggerAtMultiples = null,
            EndsAt? endsAt = null,
            bool? separateDurations = null,
            bool? canResetDuration = null) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
                endsAt ?? EndsAt.Death,
                Alignment.Neutral,
                canResetDuration: canResetDuration)
        {
            MaxAmount = maxAmount;
            TriggerAmount = triggerAmount;
            TriggeredEffects = triggeredEffects;
            SelfRemoveAfterAmountReached = selfRemoveAfterAmountReached ?? false;
            TriggerAtMultiples = triggerAtMultiples ?? false;
            SeparateDurations = separateDurations ?? false;
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
        
        public IList<EffectId> TriggeredEffects { get; }

        /// <summary>
        /// If true, each count has its own duration (controlled by <see cref="Behaviour.EndsAt"/>) and
        /// <see cref="Behaviour.CanResetDuration"/> cannot be true; if false, all counts share the same duration. 
        /// </summary>
        public bool SeparateDurations { get; }
    }
}