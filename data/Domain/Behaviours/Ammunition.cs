using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// Intercepts selected attack types to check if there's enough ammunition to execute the attack, if so
    /// reduces the ammunition amount and applies additional effects.
    /// </summary>
    public class Ammunition : Behaviour
    {
        public Ammunition(
            BehaviourId id, 
            string displayName, 
            string description,
            int maxAmmunitionAmount,
            IList<Attacks> ammunitionAttackTypes,
            int? ammunitionAmountLostOnHit = null,
            IList<EffectId>? onHitEffects = null,
            int? ammunitionRecoveredOnReload = null, 
            bool? applyOriginalAttackToTarget = null) 
            : base(
                id, 
                $"{nameof(Behaviour)}.{nameof(Ammunition)}", 
                displayName, 
                description, 
                EndsAt.Death,
                Alignment.Neutral)
        {
            MaxAmmunitionAmount = maxAmmunitionAmount;
            AmmunitionAmountLostOnHit = ammunitionAmountLostOnHit ?? 1;
            AmmunitionRecoveredOnReload = ammunitionRecoveredOnReload ?? maxAmmunitionAmount;
            ApplyOriginalAttackToTarget = applyOriginalAttackToTarget ?? true;
            AmmunitionAttackTypes = ammunitionAttackTypes;
            OnHitEffects = onHitEffects ?? new List<EffectId>();
        }
        
        public int MaxAmmunitionAmount { get; }
        public int AmmunitionAmountLostOnHit { get; }
        public int AmmunitionRecoveredOnReload { get; }
        public bool ApplyOriginalAttackToTarget { get; }
        public IList<Attacks> AmmunitionAttackTypes { get; }
        public IList<EffectId> OnHitEffects { get; }
    }
}