using System.Collections.Generic;
using low_age_data.Domain.Effects;
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
            bool destroyOnConditionsMet = false,
            IList<EffectName>? conditionalEffects = null,
            bool restoreChangesOnEnd = false) : base(name, $"{nameof(Behaviour)}.{nameof(Buff)}", displayName, description, endsAt ?? EndsAt.Death)
        {
            ModificationFlags = modificationFlags ?? new List<Flag>();
            InitialModifications = initialModifications ?? new List<Modification>();
            InitialEffects = initialEffects ?? new List<EffectName>();
            FinalModifications = finalModifications ?? new List<Modification>();
            FinalEffects = finalEffects ?? new List<EffectName>();
            CanStack = canStack ?? false;
            CanResetDuration = canResetDuration ?? false;
            Alignment = alignment ?? Alignment.Neutral;
            Triggers = triggers ?? new List<Trigger>();
            DestroyOnConditionsMet = destroyOnConditionsMet;
            ConditionalEffects = conditionalEffects ?? new List<EffectName>();
            RestoreChangesOnEnd = restoreChangesOnEnd;
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
        /// Added right before the end of behaviour.
        /// </summary>
        public IList<Modification> FinalModifications { get; }
        
        /// <summary>
        /// Executed right before behaviour ends.
        /// </summary>
        public IList<EffectName> FinalEffects { get; }
        
        /// <summary>
        /// If true, multiple <see cref="Buff"/>s can be added.
        /// </summary>
        public bool CanStack { get; }
        
        /// <summary>
        /// If true, applying the same <see cref="Buff"/> will reset the duration (to all stacks, if
        /// <see cref="CanStack"/> is true).
        /// </summary>
        public bool CanResetDuration { get; }
        public Alignment Alignment { get; }
        
        /// <summary>
        /// Logical <b>OR</b> between the <see cref="Triggers"/>, but <b>AND</b> between the <see cref="Event"/>s inside.
        /// </summary>
        public IList<Trigger> Triggers { get; }
        
        /// <summary>
        /// If true, behaviour is removed (without triggering <see cref="FinalModifications"/> or <see cref="FinalEffects"/>)
        /// when all <see cref="Event"/>s and their conditions are met in any of the <see cref="Triggers"/>.
        /// </summary>
        public bool DestroyOnConditionsMet { get; }
        
        /// <summary>
        /// Executed when any of the <see cref="Triggers"/> condition is met (before destroy of this behaviour, if
        /// <see cref="DestroyOnConditionsMet"/> is true).
        /// </summary>
        public IList<EffectName> ConditionalEffects { get; }
        
        /// <summary>
        /// If true, counter-acts the <see cref="InitialModifications"/> and <see cref="ModificationFlags"/> before
        /// end or destroy.
        /// </summary>
        public bool RestoreChangesOnEnd { get; }
    }
}
