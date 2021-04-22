using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Abilities
{
    public class Target : Ability
    {
        public Target(
            AbilityName name,
            TurnPhase turnPhase, 
            string displayName, 
            string description,
            int distance,
            EffectName effect,
            IList<Research>? researchNeeded = null,
            EndsAt? cooldown = null) : base(name, $"{nameof(Ability)}.{nameof(Target)}", turnPhase, researchNeeded ?? new List<Research>(), true, displayName, description, cooldown)
        {
            Distance = distance;
            Effect = effect;
        }

        public int Distance { get; }
        public EffectName Effect { get; }
    }
}
