using System.Collections.Generic;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Common.Modifications;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Behaviours
{
    public class Buff : Behaviour
    {
        public Buff(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            IList<ModificationFlag>? modificationFlags = null,
            IList<Modification>? initialModifications = null,
            IList<EffectId>? initialEffects = null,
            IList<Modification>? finalModifications = null,
            IList<EffectId>? finalEffects = null,
            EndsAt? endsAt = null,
            bool? canStack = null,
            bool? canResetDuration = null,
            Alignment? alignment = null,
            IList<Trigger>? triggers = null,
            bool? removeOnConditionsMet = null,
            IList<EffectId>? conditionalEffects = null,
            bool? restoreChangesOnEnd = null,
            bool? ownerAllowed = null,
            bool? hasSameInstanceForAllOwners = null) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
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
            ModificationFlags = modificationFlags ?? new List<ModificationFlag>();
            InitialModifications = initialModifications ?? new List<Modification>();
            InitialEffects = initialEffects ?? new List<EffectId>();
            FinalModifications = finalModifications ?? new List<Modification>();
            FinalEffects = finalEffects ?? new List<EffectId>();
            RestoreChangesOnEnd = restoreChangesOnEnd ?? false;
        }

        public IList<ModificationFlag> ModificationFlags { get; }
        
        /// <summary>
        /// Added at the start of behaviour.
        /// </summary>
        public IList<Modification> InitialModifications { get; }
        
        /// <summary>
        /// Executed when behaviour is added.
        /// </summary>
        public IList<EffectId> InitialEffects { get; }
        
        /// <summary>
        /// Added right before the <see cref="EndsAt"/> or before <see cref="Entity"/> is destroyed.
        /// </summary>
        public IList<Modification> FinalModifications { get; }
        
        /// <summary>
        /// Executed right before the <see cref="EndsAt"/> or before <see cref="Entity"/> is destroyed.
        /// </summary>
        public IList<EffectId> FinalEffects { get; }

        /// <summary>
        /// If true, counter-acts the <see cref="InitialModifications"/> and <see cref="ModificationFlags"/> before
        /// end or destroy.
        /// </summary>
        public bool RestoreChangesOnEnd { get; }
    }
}
