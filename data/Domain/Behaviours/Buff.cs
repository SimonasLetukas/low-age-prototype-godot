﻿using System.Collections.Generic;
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
        public IList<Modification> InitialModifications { get; } // Added at the start of behaviour
        public IList<EffectName> InitialEffects { get; } // Executed when behaviour is added
        public IList<Modification> FinalModifications { get; } // Added right before the end of behaviour
        public IList<EffectName> FinalEffects { get; } // Executed right before behaviour ends
        public bool CanStack { get; }
        public bool CanResetDuration { get; } // If true, applying the same buff will reset the duration (to all stacks,
                                              // if CanStack is true).
        public Alignment Alignment { get; }
        public IList<Trigger> Triggers { get; } // Logic OR between the triggers, but AND between events inside.
        public bool DestroyOnConditionsMet { get; } // If true, behaviour is removed (without triggering final modifications
                                                    // or effects) when all events & their conditions are met in any of the trigger
        public IList<EffectName> ConditionalEffects { get; } // Executed when all trigger conditions are met (before destroy)
        public bool RestoreChangesOnEnd { get; } // If true, counter-acts the initial modifications before end or destroy
    }
}
