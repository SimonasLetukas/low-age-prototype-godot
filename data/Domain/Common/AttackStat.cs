using Newtonsoft.Json;

namespace LowAgeData.Domain.Common
{
    public class AttackStat : Stat
    {
        [JsonConstructor]
        public AttackStat(
            int maxAmount,
            AttackType attackType, 
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

        public AttackType AttackType { get; }
        public int MinimumDistance { get; }
        public int MaximumDistance { get; }
        public ActorAttribute? BonusTo { get; }
        public int BonusAmount { get; }
    }
}
