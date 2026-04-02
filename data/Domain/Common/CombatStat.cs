namespace LowAgeData.Domain.Common
{
    public class CombatStat : Stat
    {
        public CombatStat(
            int maxAmount, 
            bool hasCurrent, 
            StatType combatType, 
            bool? allowsOverflow = null) 
            : base(maxAmount, hasCurrent, allowsOverflow)
        {
            CombatType = combatType;
        }

        public StatType CombatType { get; }
    }
}
