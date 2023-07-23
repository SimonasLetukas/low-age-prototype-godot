using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Abilities
{
    public class Instant : Ability
    {
        public Instant(
            AbilityId id,
            TurnPhase turnPhase,
            string displayName,
            string description,
            IList<EffectId> effects,
            EndsAt? cooldown = null,
            IList<ResearchId>? researchNeeded = null,
            IList<Payment>? cost = null)
            : base(
                id,
                $"{nameof(Ability)}.{nameof(Instant)}",
                turnPhase,
                researchNeeded ?? new List<ResearchId>(),
                true,
                displayName,
                description,
                cooldown,
                cost)
        {
            Effects = effects;
        }

        public IList<EffectId> Effects { get; }
    }
}