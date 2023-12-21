namespace low_age_data.Domain.Common.Modifications
{
    public class StatModification : Modification
    {
        public StatModification(
            Change change, 
            float amount,
            StatType statType) : base(change, amount)
        {
            StatType = statType;
        }

        public StatType StatType { get; }
    }
}
