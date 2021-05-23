using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Abilities
{
    public class Passive : Ability
    {
        public Passive(
            AbilityName name, 
            string displayName, 
            string description,
            bool hasButton = false,
            EffectName? periodicEffect = null,
            IList<Research>? researchNeeded = null,
            IList<EffectName>? onHitEffects = null,
            IList<Attacks>? onHitAttackTypes = null,
            EffectName? onBirthEffect = null) : base(name, $"{nameof(Ability)}.{nameof(Passive)}", TurnPhase.Passive, researchNeeded ?? new List<Research>(), hasButton, displayName, description)
        {
            PeriodicEffect = periodicEffect;
            OnHitEffects = onHitEffects ?? new List<EffectName>();
            OnHitAttackTypes = onHitAttackTypes ?? new List<Attacks>();
            OnBirthEffect = onBirthEffect;
        }

        public EffectName? PeriodicEffect { get; } // Executes effect as often as possible
        public IList<EffectName> OnHitEffects { get; } // Executes effects on each attack
        public IList<Attacks> OnHitAttackTypes { get; } // Determines which attack types trigger the on-hit effect
        public EffectName? OnBirthEffect { get; } // Executes effect upon birth or upon research completion on an entity with this ability
    }
}
