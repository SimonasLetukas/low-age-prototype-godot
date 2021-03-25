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
            EffectName effect) : base(name, $"{nameof(Ability)}.{nameof(Target)}", turnPhase, displayName, description)
        {
            Distance = distance;
            Effect = effect;
        }

        public int Distance { get; }
        public EffectName Effect { get; }
    }
}
