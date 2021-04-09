using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Effects
{
    public class Damage : Effect
    {
        public Damage(
            EffectName name,
            DamageType damageType,
            int amount = 0,
            CombatAttributes? bonusTo = null,
            int bonusAmount = 0) : base(name, $"{nameof(Effect)}.{nameof(Damage)}")
        {
            DamageType = damageType;
            Amount = amount;
            BonusTo = bonusTo;
            BonusAmount = bonusAmount;
        }

        public DamageType DamageType { get; }
        public int Amount { get; }
        public CombatAttributes? BonusTo { get; }
        public int BonusAmount { get; }
    }
}
