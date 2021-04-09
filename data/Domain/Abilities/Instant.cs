using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;

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
            IList<Research>? researchNeeded = null) : base(name, $"{nameof(Ability)}.{nameof(Instant)}", turnPhase, researchNeeded ?? new List<Research>(), true, displayName, description)
        {
            Effects = effects;
        }

        public IList<EffectName> Effects { get; }
    }
}
