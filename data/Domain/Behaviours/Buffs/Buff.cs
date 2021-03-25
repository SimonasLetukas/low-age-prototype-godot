using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Conditions;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours.Buffs
{
    public class Buff : Behaviour
    {
        public Buff(
            BehaviourName name,
            string displayName, 
            string description,
            IList<Modification>? modifications = null,
            IList<EffectName>? initialEffects = null,
            IList<EffectName>? finalEffects = null,
            EndsAt? endsAt = null,
            Alignment? alignment = null,
            IList<Condition>? conditions = null,
            IList<EffectName>? conditionalEffects = null,
            bool restoreChangesOnEnd = false) : base(name, $"{nameof(Behaviour)}.{nameof(Buff)}", displayName, description)
        {
            Modifications = modifications ?? new List<Modification>();
            InitialEffects = initialEffects ?? new List<EffectName>();
            FinalEffects = finalEffects ?? new List<EffectName>();
            EndsAt = endsAt ?? EndsAt.Death;
            Alignment = alignment ?? Alignment.Neutral;
            Conditions = conditions ?? new List<Condition>();
            ConditionalEffects = conditionalEffects ?? new List<EffectName>();
            RestoreChangesOnEnd = restoreChangesOnEnd;
        }

        public IList<Modification> Modifications { get; }
        public IList<EffectName> InitialEffects { get; } // Executed when behaviour is added
        public IList<EffectName> FinalEffects { get; } // Executed right before behaviour ends
        public EndsAt EndsAt { get; }
        public Alignment Alignment { get; }
        public IList<Condition> Conditions { get; }
        public IList<EffectName> ConditionalEffects { get; } // Executed when all conditions are met
        public bool RestoreChangesOnEnd { get; } // If true, counter-acts the modifications before expiration
    }
}
