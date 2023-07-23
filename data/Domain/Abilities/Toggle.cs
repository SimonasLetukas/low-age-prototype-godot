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
            AbilityId id,
            TurnPhase turnPhase,
            string displayName,
            string activationDescription,
            IList<EffectId> onActivatedEffects,
            IList<EffectId> onDeactivatedEffects,
            EndsAt? cooldown = null,
            IList<ResearchId>? researchNeeded = null,
            string? deactivationDescription = null,
            IList<Payment>? cost = null)
            : base(
                id,
                $"{nameof(Ability)}.{nameof(Toggle)}",
                turnPhase,
                researchNeeded ?? new List<ResearchId>(),
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
        public IList<EffectId> OnActivatedEffects { get; }
        
        /// <summary>
        /// <see cref="Effect"/>s executed when the <see cref="Toggle"/> is deactivated.
        /// </summary>
        public IList<EffectId> OnDeactivatedEffects { get; }
        
        /// <summary>
        /// <see cref="Ability.Description"/> shown when the <see cref="Toggle"/> is activated.
        /// </summary>
        public string DeactivationDescription { get; }
    }
}