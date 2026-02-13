using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Entities.Actors;

namespace LowAgeData.Domain.Behaviours
{
    /// <summary>
    /// A <see cref="Behaviour"/> that intercepts the initial incoming <see cref="Damage"/> (before armour) and
    /// acts on it in various ways. 
    /// </summary>
    public class InterceptDamage : Behaviour
    {
        public InterceptDamage(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            EndsAt endsAt,
            Alignment? alignment = null,
            int? numberOfInterceptions = null,
            IList<DamageType>? interceptedDamageTypes = null,
            Amount? amountDealtInstead = null,
            DamageType? damageTypeDealtInstead = null,
            Location? shareWith = null,
            Amount? amountShared = null,
            DamageType? damageTypeShared = null,
            bool? reduceByTheSharedAmount = null,
            bool? ownerAllowed = null,
            bool? hasSameInstanceForAllOwners = null) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
                endsAt,
                alignment ?? Alignment.Neutral,
                ownerAllowed: ownerAllowed, 
                hasSameInstanceForAllOwners: hasSameInstanceForAllOwners)
        {
            NumberOfInterceptions = numberOfInterceptions ?? -1;
            InterceptedDamageTypes = interceptedDamageTypes 
                                     ?? [DamageType.Melee, DamageType.Ranged, DamageType.Pure];
            AmountDealtInstead = amountDealtInstead;
            DamageTypeDealtInstead = damageTypeDealtInstead;
            ShareWith = shareWith;
            AmountShared = amountShared;
            DamageTypeShared = damageTypeShared;
            ReduceByTheSharedAmount = reduceByTheSharedAmount ?? false;
        }
        
        /// <summary>
        /// How many interceptions are executed until this <see cref="Behaviour"/> is removed. Value of '-1' means that
        /// there is no limit of interceptions. 
        /// </summary>
        public int NumberOfInterceptions { get; }
        
        /// <summary>
        /// List of <see cref="DamageType"/>s that can trigger this <see cref="Behaviour"/>.
        ///
        /// Default = [<see cref="DamageType.Melee"/>, <see cref="DamageType.Ranged"/>, <see cref="DamageType.Pure"/>]
        /// (all main damage types). 
        /// </summary>
        public IList<DamageType> InterceptedDamageTypes { get; }
        
        /// <summary>
        /// Specifies the <see cref="Amount"/> of damage this <see cref="Actor"/> receives instead of the intercepted
        /// <see cref="Amount"/>. Can be set to deal <see cref="Amount.Flat"/> or a <see cref="Amount.Multiplier"/>
        /// of the intercepted <see cref="Amount"/>.
        ///
        /// Note: In <see cref="Amount"/>.<see cref="Amount.MultiplyTarget"/> the '<see cref="Location.Inherited"/>'
        /// value means that the incoming damage is taken as the multiply target. 
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