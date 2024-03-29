﻿using System.Collections.Generic;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Durations;
using low_age_data.Domain.Effects;

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
            string sprite,
            int maxAmmunitionAmount,
            IList<Attacks> ammunitionAttackTypes,
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
        public IList<Attacks> AmmunitionAttackTypes { get; }
        public IList<EffectId> OnHitEffects { get; }
    }
}