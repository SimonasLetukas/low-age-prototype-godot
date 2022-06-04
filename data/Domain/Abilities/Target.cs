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
            IList<EffectName> effects,
            IList<Research>? researchNeeded = null,
            EndsAt? cooldown = null) : base(name, $"{nameof(Ability)}.{nameof(Target)}", turnPhase, researchNeeded ?? new List<Research>(), true, displayName, description, cooldown)
        {
            Distance = distance;
            Effects = effects;
        }

        public int Distance { get; }
        public IList<EffectName> Effects { get; }
    }
}
