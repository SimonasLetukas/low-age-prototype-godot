using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Effects;

namespace LowAgeData.Domain.Behaviours
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
            string sprite,
            int maxAmmunitionAmount,
            IList<AttackType> ammunitionAttackTypes,
            int? ammunitionAmountLostOnHit = null,
            IList<EffectId>? onHitEffects = null,
            int? ammunitionRecoveredOnReload = null, 
            bool? applyOriginalAttackToTarget = null) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
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
        public IList<AttackType> AmmunitionAttackTypes { get; }
        public IList<EffectId> OnHitEffects { get; }
    }
}