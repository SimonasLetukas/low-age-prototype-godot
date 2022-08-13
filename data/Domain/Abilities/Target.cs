using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Logic;
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
            EndsAt? cooldown = null,
            IList<Attacks>? overridesAttacks = null,
            bool? fallbackToAttack = false) : base(name, $"{nameof(Ability)}.{nameof(Target)}", turnPhase, researchNeeded ?? new List<Research>(), true, displayName, description, cooldown)
        {
            Distance = distance;
            Effects = effects;
            OverridesAttacks = overridesAttacks ?? new List<Attacks>();
            FallbackToAttack = fallbackToAttack ?? false;
        }

        public int Distance { get; }
        public IList<EffectName> Effects { get; }
        
        /// <summary>
        /// Attacking using any of the listed <see cref="Attacks"/> will execute this <see cref="Target"/> ability
        /// instead. Having any <see cref="Attacks"/> in this list will automatically change button of
        /// <see cref="Target"/> to be <see cref="Passive"/>. <see cref="Attacks"/> could then target ground, depending
        /// on underlying <see cref="Effects"/>.
        /// </summary>
        public IList<Attacks> OverridesAttacks { get; }
        
        /// <summary>
        /// If true, when <see cref="Target"/> cannot override an attack listed in <see cref="OverridesAttacks"/> (e.g.
        /// when <see cref="Distance"/>, <see cref="Ability.Cooldown"/>, <see cref="Research"/> or any
        /// <see cref="Validator"/> in underlying <see cref="Effects"/> is not satisfied), the attack will be executed
        /// instead. 
        /// </summary>
        public bool FallbackToAttack { get; }
    }
}
