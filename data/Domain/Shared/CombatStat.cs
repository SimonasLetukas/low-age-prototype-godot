namespace low_age_data.Domain.Shared
{
    public class CombatStat : Stat
    {
        public CombatStat(int maxAmount, bool hasCurrent, Stats combatType) : base(maxAmount, hasCurrent)
        {
            CombatType = combatType;
        }

        public Stats CombatType { get; }
    }
}
