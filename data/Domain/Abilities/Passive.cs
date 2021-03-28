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
            EffectName? periodicEffect = null,
            IList<Research>? researchNeeded = null,
            EffectName? onHitEffect = null,
            IList<Attacks>? onHitAttackTypes = null) : base(name, $"{nameof(Ability)}.{nameof(Passive)}", TurnPhase.Passive, researchNeeded ?? new List<Research>(), displayName, description)
        {
            PeriodicEffect = periodicEffect;
            OnHitEffect = onHitEffect;
            OnHitAttackTypes = onHitAttackTypes ?? new List<Attacks>();
        }

        public EffectName? PeriodicEffect { get; } // Executes effect as often as possible
        public EffectName? OnHitEffect { get; } // Executes effect on each attack
        public IList<Attacks> OnHitAttackTypes { get; } // Determines which attack types trigger the on-hit effect
    }
}
