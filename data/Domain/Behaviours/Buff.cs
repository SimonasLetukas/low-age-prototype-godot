using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared.Conditions;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    public class Buff : Behaviour
    {
        public Buff(
            BehaviourName name,
            string displayName, 
            string description,
            IList<EffectName>? initialEffects = null,
            IList<EffectName>? finalEffects = null,
            EndsAt? endsAt = null,
            IList<Condition>? conditions = null,
            IList<EffectName>? conditionalEffects = null) : base(name, $"{nameof(Behaviour)}.{nameof(Buff)}", displayName, description)
        {
            InitialEffects = initialEffects ?? new List<EffectName>();
            FinalEffects = finalEffects ?? new List<EffectName>();
            EndsAt = endsAt ?? EndsAt.Death;
            Conditions = conditions ?? new List<Condition>();
            ConditionalEffects = conditionalEffects ?? new List<EffectName>();
        }

        public IList<EffectName> InitialEffects { get; } // Executed when behaviour is added
        public IList<EffectName> FinalEffects { get; } // Executed right before behaviour ends
        public EndsAt EndsAt { get; }
        public IList<Condition> Conditions { get; }
        public IList<EffectName> ConditionalEffects { get; } // Executed when all conditions are met
    }
}
