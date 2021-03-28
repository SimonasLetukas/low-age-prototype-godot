using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;

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
            IList<Research>? researchNeeded = null) : base(name, $"{nameof(Ability)}.{nameof(Target)}", turnPhase, researchNeeded ?? new List<Research>(), displayName, description)
        {
            Distance = distance;
            Effect = effect;
        }

        public int Distance { get; }
        public EffectName Effect { get; }
    }
}
