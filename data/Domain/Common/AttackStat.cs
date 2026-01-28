using Newtonsoft.Json;

namespace LowAgeData.Domain.Common
{
    public class AttackStat : Stat
    {
        [JsonConstructor]
        public AttackStat(
            string displayName,
            int maxAmount,
            AttackType attackType, 
            int minimumDistance, 
            int maximumDistance, 
            ActorAttribute? bonusTo = null, 
            int bonusAmount = 0) : base(maxAmount, false)
        {
            DisplayName = displayName;
            AttackType = attackType;
            MinimumDistance = minimumDistance;
            MaximumDistance = maximumDistance;
            BonusTo = bonusTo;
            BonusAmount = bonusAmount;
        }

        public string DisplayName { get; }
        public AttackType AttackType { get; }
        public int MinimumDistance { get; }
        public int MaximumDistance { get; }
        public ActorAttribute? BonusTo { get; }
        public int BonusAmount { get; }
    }
}
