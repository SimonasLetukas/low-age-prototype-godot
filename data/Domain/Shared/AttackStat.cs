namespace low_age_data.Domain.Shared
{
    public class AttackStat : Stat
    {
        public AttackStat(
            int maxAmount, 
            bool hasCurrent, 
            Attacks attackType, 
            int minimumDistance, 
            int maximumDistance, 
            CombatAttributes? bonusTo = null, 
            int bonusAmount = 0) : base(maxAmount, hasCurrent)
        {
            AttackType = attackType;
            MinimumDistance = minimumDistance;
            MaximumDistance = maximumDistance;
            BonusTo = bonusTo;
            BonusAmount = bonusAmount;
        }

        public Attacks AttackType { get; }
        public int MinimumDistance { get; }
        public int MaximumDistance { get; }
        public CombatAttributes? BonusTo { get; }
        public int BonusAmount { get; }
    }
}
