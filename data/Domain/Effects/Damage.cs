using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Effects
{
    public class Damage : Effect
    {
        public Damage(
            EffectId id,
            DamageType damageType,
            Amount? amount = null,
            ActorAttribute? bonusTo = null,
            Amount? bonusAmount = null,
            Location? location = null,
            bool? ignoresArmor = null,
            bool? ignoresShield = null,
            IList<IFilterItem>? filters = null,
            IList<Validator>? validators = null) : base(id, validators ?? new List<Validator>())
        {
            DamageType = damageType;
            Amount = amount ?? new Amount(0);
            BonusTo = bonusTo;
            BonusAmount = bonusAmount ?? new Amount(0);
            Location = location ?? Location.Inherited;
            IgnoresArmor = ignoresArmor ?? false;
            IgnoresShield = ignoresShield ?? false;
            Filters = filters ?? new List<IFilterItem>();
        }
        
        public DamageType DamageType { get; }
        public Amount Amount { get; }
        public ActorAttribute? BonusTo { get; }
        public Amount BonusAmount { get; }
        public Location Location { get; }
        public bool IgnoresArmor { get; }
        public bool IgnoresShield { get; }
        public IList<IFilterItem> Filters { get; }
    }
}
