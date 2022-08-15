using System.Collections.Generic;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Effects
{
    public class Damage : Effect
    {
        public Damage(
            EffectName name,
            DamageType damageType,
            Amount? amount = null,
            CombatAttributes? bonusTo = null,
            Amount? bonusAmount = null,
            Location? location = null,
            IList<Flag>? filterFlags = null,
            IList<Validator>? validators = null) : base(name, $"{nameof(Effect)}.{nameof(Damage)}", validators ?? new List<Validator>())
        {
            DamageType = damageType;
            Amount = amount ?? new Amount(0);
            BonusTo = bonusTo;
            BonusAmount = bonusAmount ?? new Amount(0);
            Location = location ?? Location.Inherited;
            FilterFlags = filterFlags ?? new List<Flag>();
        }
        
        public DamageType DamageType { get; }
        public Amount Amount { get; }
        public CombatAttributes? BonusTo { get; }
        public Amount BonusAmount { get; }
        public Location Location { get; }
        public IList<Flag> FilterFlags { get; }
    }
}
