namespace low_age_data.Domain.Common
{
    public class AttackStat : Stat
    {
        public AttackStat(
            int maxAmount,
            Attacks attackType, 
            int minimumDistance, 
            int maximumDistance, 
            ActorAttribute? bonusTo = null, 
            int bonusAmount = 0) : base(maxAmount, false)
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
        public ActorAttribute? BonusTo { get; }
        public int BonusAmount { get; }
    }
}
