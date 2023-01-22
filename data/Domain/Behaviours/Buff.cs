using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;
using low_age_data.Domain.Shared.Flags;
using low_age_data.Domain.Shared.Modifications;

namespace low_age_data.Domain.Behaviours
{
    public class Buff : Behaviour
    {
        public Buff(
            BehaviourName name,
            string displayName, 
            string description,
            IList<Flag>? modificationFlags = null,
            IList<Modification>? initialModifications = null,
            IList<EffectName>? initialEffects = null,
            IList<Modification>? finalModifications = null,
            IList<EffectName>? finalEffects = null,
            EndsAt? endsAt = null,
            bool? canStack = null,
            bool? canResetDuration = null,
            Alignment? alignment = null,
            IList<Trigger>? triggers = null,
            bool? removeOnConditionsMet = null,
            IList<EffectName>? conditionalEffects = null,
            bool? restoreChangesOnEnd = null,
            bool? ownerAllowed = null,
            bool? hasSameInstanceForAllOwners = null) 
            : base(
                name, 
                $"{nameof(Behaviour)}.{nameof(Buff)}", 
                displayName, 
                description, 
                endsAt ?? EndsAt.Death,
                alignment ?? Alignment.Neutral,
                canStack,
                canResetDuration,
                triggers,
                removeOnConditionsMet,
                conditionalEffects,
                ownerAllowed, 
                hasSameInstanceForAllOwners)
        {
            ModificationFlags = modificationFlags ?? new List<Flag>();
            InitialModifications = initialModifications ?? new List<Modification>();
            InitialEffects = initialEffects ?? new List<EffectName>();
            FinalModifications = finalModifications ?? new List<Modification>();
            FinalEffects = finalEffects ?? new List<EffectName>();
            RestoreChangesOnEnd = restoreChangesOnEnd ?? false;
        }

        public IList<Flag> ModificationFlags { get; }
        
        /// <summary>
        /// Added at the start of behaviour.
        /// </summary>
        public IList<Modification> InitialModifications { get; }
        
        /// <summary>
        /// Executed when behaviour is added.
        /// </summary>
        public IList<EffectName> InitialEffects { get; }
        
        /// <summary>
        /// Added right before the <see cref="EndsAt"/> or before <see cref="Entity"/> is destroyed.
        /// </summary>
        public IList<Modification> FinalModifications { get; }
        
        /// <summary>
        /// Executed right before the <see cref="EndsAt"/> or before <see cref="Entity"/> is destroyed.
        /// </summary>
        public IList<EffectName> FinalEffects { get; }

        /// <summary>
        /// If true, counter-acts the <see cref="InitialModifications"/> and <see cref="ModificationFlags"/> before
        /// end or destroy.
        /// </summary>
        public bool RestoreChangesOnEnd { get; }
    }
}
