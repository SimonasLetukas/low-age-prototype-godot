using System.Collections.Generic;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Effects;

namespace LowAgeData.Domain.Abilities
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