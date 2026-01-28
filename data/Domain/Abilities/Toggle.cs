using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Effects;

namespace LowAgeData.Domain.Abilities
{
    /// <summary>
    /// <para>
    /// Used when the <see cref="Ability"/> should have two modes: activated and deactivated (default).
    /// </para>
    /// <para>
    /// <see cref="Ability.Cooldown"/> is used to control activation only. <see cref="Ability.Cost"/> also is
    /// paid for the activation only.
    /// </para>
    /// </summary>
    public class Toggle : Ability
    {
        public Toggle(
            AbilityId id,
            TurnPhase turnPhase,
            string displayName,
            string activationDescription,
            string sprite,
            IList<EffectId> onActivatedEffects,
            IList<EffectId> onDeactivatedEffects,
            EndsAt? cooldown = null,
            IList<ResearchId>? researchNeeded = null,
            string? deactivationDescription = null,
            IList<Payment>? cost = null)
            : base(
                id,
                turnPhase,
                researchNeeded ?? new List<ResearchId>(),
                true,
                displayName,
                activationDescription,
                sprite,
                cooldown,
                cost)
        {
            OnActivatedEffects = onActivatedEffects;
            OnDeactivatedEffects = onDeactivatedEffects;
            DeactivationDescription = deactivationDescription ?? activationDescription;
        }
        
        /// <summary>
        /// <see cref="Effect"/>s executed after the <see cref="Toggle"/> is activated and
        /// the <see cref="Ability.Cost"/> is paid.
        /// </summary>
        public IList<EffectId> OnActivatedEffects { get; }
        
        /// <summary>
        /// <see cref="Effect"/>s executed after the <see cref="Toggle"/> is deactivated.
        /// </summary>
        public IList<EffectId> OnDeactivatedEffects { get; }
        
        /// <summary>
        /// <see cref="Ability.Description"/> shown when the <see cref="Toggle"/> is activated.
        /// </summary>
        public string DeactivationDescription { get; }
    }
}