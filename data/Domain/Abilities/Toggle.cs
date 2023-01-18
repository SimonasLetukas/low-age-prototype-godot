using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Abilities
{
    /// <summary>
    /// Used when the <see cref="Ability"/> should have two modes: activated and deactivated (default).
    ///
    /// <see cref="Ability.Cooldown"/> is used to control activation only. 
    /// </summary>
    public class Toggle : Ability
    {
        public Toggle(
            AbilityName name,
            TurnPhase turnPhase,
            string displayName,
            string activationDescription,
            IList<EffectName> onActivatedEffects,
            IList<EffectName> onDeactivatedEffects,
            EndsAt? cooldown = null,
            IList<ResearchName>? researchNeeded = null,
            string? deactivationDescription = null,
            IList<Cost>? cost = null)
            : base(
                name,
                $"{nameof(Ability)}.{nameof(Toggle)}",
                turnPhase,
                researchNeeded ?? new List<ResearchName>(),
                true,
                displayName,
                activationDescription,
                cooldown,
                cost)
        {
            OnActivatedEffects = onActivatedEffects;
            OnDeactivatedEffects = onDeactivatedEffects;
            DeactivationDescription = deactivationDescription ?? activationDescription;
        }
        
        /// <summary>
        /// <see cref="Effect"/>s executed when the <see cref="Toggle"/> is activated.
        /// </summary>
        public IList<EffectName> OnActivatedEffects { get; }
        
        /// <summary>
        /// <see cref="Effect"/>s executed when the <see cref="Toggle"/> is deactivated.
        /// </summary>
        public IList<EffectName> OnDeactivatedEffects { get; }
        
        /// <summary>
        /// <see cref="Ability.Description"/> shown when the <see cref="Toggle"/> is activated.
        /// </summary>
        public string DeactivationDescription { get; }
    }
}