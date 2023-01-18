using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Abilities
{
    public class Instant : Ability
    {
        public Instant(
            AbilityName name,
            TurnPhase turnPhase,
            string displayName,
            string description,
            IList<EffectName> effects,
            EndsAt? cooldown = null,
            IList<ResearchName>? researchNeeded = null,
            IList<Cost>? cost = null)
            : base(
                name,
                $"{nameof(Ability)}.{nameof(Instant)}",
                turnPhase,
                researchNeeded ?? new List<ResearchName>(),
                true,
                displayName,
                description,
                cooldown,
                cost)
        {
            Effects = effects;
        }

        public IList<EffectName> Effects { get; }
    }
}