using low_age_data.Domain.Effects;

namespace low_age_data.Domain.Abilities
{
    public class Passive : Ability
    {
        public Passive(
            AbilityName name, 
            string displayName, 
            string description,
            EffectName effect) : base(name, $"{nameof(Ability)}.{nameof(Passive)}", displayName, description)
        {
            Effect = effect;
        }

        public EffectName Effect { get; } // Executes effect as often as possible
    }
}
