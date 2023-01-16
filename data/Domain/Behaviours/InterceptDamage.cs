using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// A <see cref="Behaviour"/> that intercepts incoming <see cref="Damage"/> and acts on it in various ways. 
    /// </summary>
    public class InterceptDamage : Behaviour
    {
        public InterceptDamage(
            BehaviourName name,
            string displayName, 
            string description,
            EndsAt endsAt,
            int numberOfInterceptions = 0,
            IList<DamageType>? damageTypes = null,
            Amount? amountDealtInstead = null,
            DamageType? damageTypeDealtInstead = null,
            Location? shareWith = null,
            Amount? amountShared = null,
            DamageType? damageTypeShared = null,
            bool? reduceByTheSharedAmount = null,
            bool? ownerAllowed = null,
            bool? hasSameInstanceForAllOwners = null) 
            : base(
                name, 
                $"{nameof(Behaviour)}.{nameof(InterceptDamage)}", 
                displayName, 
                description, 
                endsAt,
                ownerAllowed, 
                hasSameInstanceForAllOwners)
        {
            NumberOfInterceptions = numberOfInterceptions;
            DamageTypes = damageTypes ?? new List<DamageType>();
            AmountDealtInstead = amountDealtInstead;
            DamageTypeDealtInstead = damageTypeDealtInstead;
            ShareWith = shareWith;
            AmountShared = amountShared;
            DamageTypeShared = damageTypeShared;
            ReduceByTheSharedAmount = reduceByTheSharedAmount ?? false;
        }
        
        /// <summary>
        /// How many interceptions are executed until this <see cref="Behaviour"/> is removed. Value of '0' means that
        /// there is no limit of interceptions. 
        /// </summary>
        public int NumberOfInterceptions { get; }
        
        /// <summary>
        /// List of <see cref="DamageType"/>s that trigger this <see cref="Behaviour"/>. An empty list means that all
        /// damage goes through <see cref="InterceptDamage"/>.
        /// </summary>
        public IList<DamageType> DamageTypes { get; }
        
        /// <summary>
        /// Specifies the <see cref="Amount"/> of damage this <see cref="Actor"/> receives instead of the intercepted
        /// <see cref="Amount"/>. Can be set to deal <see cref="Amount.Flat"/> or a <see cref="Amount.Multiplier"/>
        /// of the intercepted <see cref="Amount"/>.
        /// </summary>
        public Amount? AmountDealtInstead { get; }
        
        /// <summary>
        /// If set, changes any outgoing damage <see cref="AmountDealtInstead"/> to the specified
        /// <see cref="DamageType"/>. Otherwise the <see cref="DamageType"/> is retained.
        /// </summary>
        public DamageType? DamageTypeDealtInstead { get; }
        
        /// <summary>
        /// Specifies who will receive the damage that the <see cref="Actor"/> with this <see cref="InterceptDamage"/>
        /// receives.
        /// </summary>
        public Location? ShareWith { get; }
        
        /// <summary>
        /// Specifies the amount of damage dealt to the <see cref="Actor"/> specified in <see cref="ShareWith"/>.
        /// <see cref="AmountShared"/> can be set to deal <see cref="Amount.Flat"/> or a <see cref="Amount.Multiplier"/>
        /// of the received damage.
        /// </summary>
        public Amount? AmountShared { get; }
        
        /// <summary>
        /// If set, changes any outgoing damage <see cref="AmountShared"/> to the specified
        /// <see cref="DamageType"/>. Otherwise the <see cref="DamageType"/> is retained.
        /// </summary>
        public DamageType? DamageTypeShared { get; }
        
        /// <summary>
        /// If true, the final damage this <see cref="Actor"/> would receive is reduced by <see cref="Amount"/>
        /// in <see cref="AmountShared"/>.
        /// </summary>
        public bool ReduceByTheSharedAmount { get; }
    }
}