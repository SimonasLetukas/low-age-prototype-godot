using System.Collections.Generic;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Durations;
using low_age_data.Domain.Effects;

namespace low_age_data.Domain.Abilities
{
    public class Instant : Ability
    {
        public Instant(
            AbilityId id,
            TurnPhase turnPhase,
            string displayName,
            string description,
            string sprite,
            IList<EffectId> effects,
            EndsAt? cooldown = null,
            IList<ResearchId>? researchNeeded = null,
            IList<Payment>? cost = null)
            : base(
                id,
                turnPhase,
                researchNeeded ?? new List<ResearchId>(),
                true,
                displayName,
                description,
                sprite,
                cooldown,
                cost)
        {
            Effects = effects;
        }

        public IList<EffectId> Effects { get; }
    }
}